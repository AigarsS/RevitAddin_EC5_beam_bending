using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.CodeChecking.Storage;
using Autodesk.Revit.DB.CodeChecking;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ResultsBuilder;
using Autodesk.Revit.DB.ResultsBuilder.Storage;

namespace RevitAddin_EC5_beam_bending
{
    [Autodesk.Revit.DB.CodeChecking.Attributes.ServerVersion(1)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.CalculationParamsStructure(typeof(CalculationParameter))]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_BeamAnalytical, StructuralAssetClass.Wood)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.ResultStructure(typeof(Result), BuiltInCategory.OST_BeamAnalytical, StructuralAssetClass.Wood)]

    public class Server : Autodesk.Revit.DB.CodeChecking.Documentation.MultiStructureServer
    {
        public static readonly Guid ID = new Guid("8e273cf2-f9a0-45b5-9e83-b1a0068d56b5");
        #region ICodeCheckingServer Members

        public override void Verify(Autodesk.Revit.DB.CodeChecking.ServiceData data)
        {

            StorageService service = StorageService.GetStorageService();
            StorageDocument storageDocument = service.GetStorageDocument(data.Document);
            CalculationParameter myParams = storageDocument.CalculationParamsManager.CalculationParams.GetEntity<CalculationParameter>(data.Document);

            List<ElementId> loadCasesAndCombinations = storageDocument.CalculationParamsManager.CalculationParams.GetLoadCasesAndCombinations(ID);
            foreach (ElementId id in loadCasesAndCombinations)
            {
                Element loadCaseCombinationElement = data.Document.GetElement(id);
                LoadCase loadCase = loadCaseCombinationElement as LoadCase;
                LoadCombination loadCombination = loadCaseCombinationElement as LoadCombination;
            }

            ResultsPackage inputPackage = storageDocument.CalculationParamsManager.CalculationParams.GetInputResultPackage(ID);
            var resultsMy = inputPackage.GetLineGraphs(data.Selection.Select(o => o.Id).ToList(), loadCasesAndCombinations, new List<LinearResultType> { LinearResultType.My, LinearResultType.Mz, LinearResultType.Fx });
            var loads = resultsMy.GroupBy(s => s.LoadId);
            double maxMy = 0;
            double maxMz = 0;
            double maxN = 0;


            StringBuilder res = new StringBuilder();

            foreach (Element element in data.Selection)
            {
                BuiltInCategory category = Tools.GetCategoryOfElement(element);
                StructuralAssetClass material = Tools.GetClassMaterialOfElement(element);
                if (material != StructuralAssetClass.Wood || category != BuiltInCategory.OST_BeamAnalytical) continue;

                Autodesk.Revit.DB.CodeChecking.Storage.Label ccLabel = storageDocument.LabelsManager.GetLabel(element);
                if (ccLabel != null)
                {
                    Label myLabel = ccLabel.GetEntity<Label>(data.Document);
                    if (myLabel != null)
                    {
                        
                        foreach (IGrouping<ElementId, LineGraph> load in loads)
                        {
                            foreach (LineGraph lineGraph in load)
                            {

                                if (lineGraph.ElementId == element.Id && lineGraph.ResultType == LinearResultType.My)
                                {
                                    maxMy = lineGraph.Points.Max(s => s.V);
                                }

                                if (lineGraph.ElementId == element.Id && lineGraph.ResultType == LinearResultType.Mz)
                                {
                                    maxMz = lineGraph.Points.Min(s => s.V);
                                }

                                if (lineGraph.ElementId == element.Id && lineGraph.ResultType == LinearResultType.Fx)
                                {
                                    maxN = lineGraph.Points.Min(s => s.V);
                                }

                                Calculate(myParams, myLabel, element, storageDocument.ResultsManager, maxMy, maxMz, maxN);
                            }
                        }
                    }
                }
            }

            ResultsPackageBuilder builder = storageDocument.CalculationParamsManager.CalculationParams.GetOutputResultPackageBuilder(ID);
            //do something here
            builder.Finish();

        }

        void Calculate(CalculationParameter parameters, Label label, Element element, ResultsManager manager, double maxMy, double maxMz, double maxN)
        {
            Result result = new Result();
            Material mat = Tools.GetMaterialOfElement(element);
            PropertySetElement propertySetElement = mat.Document.GetElement(mat.StructuralAssetId) as PropertySetElement;
            StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();
            result.fmk = UnitUtils.ConvertFromInternalUnits(structuralAsset.WoodBendingStrength, DisplayUnitType.DUT_PASCALS);
            result.fc0k = UnitUtils.ConvertFromInternalUnits(structuralAsset.WoodParallelCompressionStrength, DisplayUnitType.DUT_PASCALS);

            AnalyticalModel analyticalModel = element as AnalyticalModel;
            FamilyInstance familyInstance = element.Document.GetElement(analyticalModel.GetElementId()) as FamilyInstance;
            Parameter parameterA = familyInstance.Symbol.GetParameters("A").First();
            Parameter parameterWy = familyInstance.Symbol.GetParameters("Wy").Max();
            Parameter parameterWz = familyInstance.Symbol.GetParameters("Wz").Max();
            

            if (parameterA != null)
            {
                result.A = UnitUtils.ConvertFromInternalUnits(parameterA.AsDouble(), DisplayUnitType.DUT_SQUARE_METERS);
                result.MyEd = Math.Abs(maxMy);
                result.MzEd = Math.Abs(maxMz);
                result.NEd = Math.Abs(maxN);
                result.Wy = UnitUtils.ConvertFromInternalUnits(parameterWy.AsDouble(),DisplayUnitType.DUT_CUBIC_CENTIMETERS);
                result.Wz = UnitUtils.ConvertFromInternalUnits(parameterWz.AsDouble(), DisplayUnitType.DUT_CUBIC_CENTIMETERS);
                result.fmd = result.fmk/parameters.gammaM;
                result.sigmaMyd = result.MyEd*1000000 / result.Wy;
                result.sigmaMzd = result.MzEd * 1000000 / result.Wz;
                result.sigmaC0d = result.NEd / result.A;


                result.Ratio = result.sigmaMyd / result.fmd + result.sigmaMzd / result.fmd + Math.Pow(result.sigmaC0d /(result.fc0k/parameters.gammaM),2);


                ResultStatus status = new ResultStatus(ID);
                status.SetStatusRatioBased(result.Ratio);
                manager.SetResult(result.GetEntity(), element, status);
            }
        }

        public override IList<BuiltInCategory> GetSupportedCategories(StructuralAssetClass material)
        {
            return new List<BuiltInCategory>() { BuiltInCategory.OST_BeamAnalytical};
        }

        public override IList<Autodesk.Revit.DB.StructuralAssetClass> GetSupportedMaterials()
        {
            return new List<Autodesk.Revit.DB.StructuralAssetClass>() { StructuralAssetClass.Wood };
        }

        public override bool LoadCasesAndCombinationsSupport()
        {
            return true;
        }

        public override bool ResultBuilderPackagesAsInputData()
        {
            return true;
        }

        public override Autodesk.Revit.DB.ResultsBuilder.UnitsSystem GetOutputPackageUnitSystem()
        {
            return Autodesk.Revit.DB.ResultsBuilder.UnitsSystem.Metric;
        }

        #endregion

        #region IExternalServer Members

        public override string GetDescription()
        {
            return "EC5 Beam Bending";
        }

        public override string GetName()
        {
            return "EC5 Beam Bending";
        }

        public override Guid GetServerId()
        {
            return ID;
        }

        public override string GetVendorId()
        {
            return "John Smith";
        }

        #endregion

        #region ICodeCheckingServerDocumentation Members     

        public override string GetResource(string key, string context)
        {
            string txt = RevitAddin_EC5_beam_bending.Properties.Resources.ResourceManager.GetString(key);

            if (!string.IsNullOrEmpty(txt))
                return txt;

            return key;
        }

        #endregion
    }
}
