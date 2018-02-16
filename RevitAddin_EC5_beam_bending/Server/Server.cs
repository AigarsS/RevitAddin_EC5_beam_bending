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
using Autodesk.Revit.DB.ExtensibleStorage.Framework.Documentation;

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

        public override void Verify(ServiceData data)
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
            var resultsMy = inputPackage.GetLineGraphs(data.Selection.Select(o => o.Id).ToList(), loadCasesAndCombinations, new List<LinearResultType> { LinearResultType.My, LinearResultType.Mz, LinearResultType.Fx, LinearResultType.Fz });
            var loads = resultsMy.GroupBy(s => s.LoadId);
            
            double elemMY = 0;
            double elemMZ = 0;
            double elemN = 0;
            double elemVZ = 0;

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
                                    double minMy = lineGraph.Points.Min(s => s.V);
                                    double maxMy = lineGraph.Points.Max(s => s.V);
                                    if (Math.Abs(minMy) > Math.Abs(maxMy)){ elemMY = Math.Abs(minMy);}
                                    else { elemMY = Math.Abs(maxMy); }
                                }

                                if (lineGraph.ElementId == element.Id && lineGraph.ResultType == LinearResultType.Mz)
                                {
                                    double minMz = lineGraph.Points.Min(s => s.V);
                                    double maxMz = lineGraph.Points.Max(s => s.V);
                                    if (Math.Abs(minMz) > Math.Abs(maxMz)) { elemMZ = Math.Abs(minMz); }
                                    else { elemMZ = Math.Abs(maxMz); }
                                }

                                if (lineGraph.ElementId == element.Id && lineGraph.ResultType == LinearResultType.Fx)
                                {
                                    double minN = lineGraph.Points.Min(s => s.V);
                                    double maxN = lineGraph.Points.Max(s => s.V);
                                    if (Math.Abs(minN) > Math.Abs(maxN)) { elemN = Math.Abs(minN); }
                                    else { elemN = Math.Abs(maxN); }
                                }

                                if (lineGraph.ElementId == element.Id && lineGraph.ResultType == LinearResultType.Fz)
                                {
                                    double minVz = lineGraph.Points.Min(s => s.V);
                                    double maxVz = lineGraph.Points.Max(s => s.V);
                                    if (Math.Abs(minVz) > Math.Abs(maxVz)) { elemVZ = Math.Abs(minVz); }
                                    else { elemVZ= Math.Abs(maxVz); }
                                }
                                Result result2 = new Result();
                                CalculateBending(result2, myParams, myLabel, element, storageDocument.ResultsManager, elemMY, elemMZ, elemN);
                                CalculateShear(result2, myParams, myLabel, element, storageDocument.ResultsManager, elemVZ);
                                
                            }
                        }
                    }
                }
            }


            Autodesk.Revit.DB.ExtensibleStorage.Framework.Documentation.Document repdocument = new Autodesk.Revit.DB.ExtensibleStorage.Framework.Documentation.Document();
            Result result = new Result();
            DocumentBody.FillBody(result, repdocument.Main, this, data.Document);
            HtmlBuilder htmlBuilder = new HtmlBuilder();
            htmlBuilder.BuildDocument(repdocument, data.Document, "C:\\tmp\\result", DetailLevel.Detail);

            ResultsPackageBuilder builder = storageDocument.CalculationParamsManager.CalculationParams.GetOutputResultPackageBuilder(ID);
            //do something here
            builder.Finish();

        }

        void CalculateBending(Result result,CalculationParameter parameters, Label label, Element element, ResultsManager manager, double elemMY, double elemMZ, double elemN)
        {
            Material mat = Tools.GetMaterialOfElement(element);
            PropertySetElement propertySetElement = mat.Document.GetElement(mat.StructuralAssetId) as PropertySetElement;
            StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();
            
            result.fmk = UnitUtils.ConvertFromInternalUnits(structuralAsset.WoodBendingStrength, DisplayUnitType.DUT_PASCALS);
            result.fc0k = UnitUtils.ConvertFromInternalUnits(structuralAsset.WoodParallelCompressionStrength, DisplayUnitType.DUT_PASCALS);

            AnalyticalModel analyticalModel = element as AnalyticalModel;
            FamilyInstance familyInstance = element.Document.GetElement(analyticalModel.GetElementId()) as FamilyInstance;
            
            Double parameterWidth = familyInstance.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH).AsDouble();
            Double parameterHeight = familyInstance.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT).AsDouble();
            Parameter paramSectionShape = familyInstance.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_SHAPE);

            //Parameter parameterA = familyInstance.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_AREA);
            //Parameter parameterWy = familyInstance.Symbol.GetParameters("Wy").Max();
            //Parameter parameterWz = familyInstance.Symbol.GetParameters("Wz").Max();

            double Wy = parameterWidth * Math.Pow(parameterHeight, 2) / 6;
            double Wz = parameterHeight * Math.Pow(parameterWidth, 2) / 6;
            double A = parameterWidth * parameterHeight;

            
            double km = 1;
            if (paramSectionShape.AsInteger() == 31)
            {
                km = 0.7;
            }

            if (A != 0)
            {
                result.A = UnitUtils.ConvertFromInternalUnits(A, DisplayUnitType.DUT_SQUARE_METERS);
                result.MyEd = Math.Abs(elemMY);
                result.MzEd = Math.Abs(elemMZ);
                result.NEd = Math.Abs(elemN);
                result.Wy = UnitUtils.ConvertFromInternalUnits(Wy,DisplayUnitType.DUT_CUBIC_CENTIMETERS);
                result.Wz = UnitUtils.ConvertFromInternalUnits(Wz, DisplayUnitType.DUT_CUBIC_CENTIMETERS);
                result.fmd = result.fmk/parameters.gammaM;
                result.sigmaMyd = result.MyEd*1000000 / result.Wy;
                result.sigmaMzd = result.MzEd * 1000000 / result.Wz;
                result.sigmaC0d = result.NEd / result.A;


                result.Ratio1 = km*result.sigmaMyd / result.fmd + result.sigmaMzd / result.fmd + Math.Pow(result.sigmaC0d /(result.fc0k/parameters.gammaM),2);
                result.Ratio2 = result.sigmaMyd / result.fmd + km * result.sigmaMzd / result.fmd + Math.Pow(result.sigmaC0d / (result.fc0k / parameters.gammaM), 2);


                ResultStatus status = new ResultStatus(ID);
                status.SetStatusRatioBased(result.Ratio1);
                manager.SetResult(result.GetEntity(), element, status);
            }
        }

        void CalculateShear(Result result, CalculationParameter parameters, Label label, Element element, ResultsManager manager, double elemVz)
        {

            Material mat = Tools.GetMaterialOfElement(element);
            PropertySetElement propertySetElement = mat.Document.GetElement(mat.StructuralAssetId) as PropertySetElement;
            StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

            result.fv0k = UnitUtils.ConvertFromInternalUnits(structuralAsset.WoodParallelShearStrength, DisplayUnitType.DUT_PASCALS);
            result.fvd = result.fv0k / parameters.gammaM;

            AnalyticalModel analyticalModel = element as AnalyticalModel;
            FamilyInstance familyInstance = element.Document.GetElement(analyticalModel.GetElementId()) as FamilyInstance;

            Double parameterWidth = familyInstance.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH).AsDouble();
            Double parameterHeight = familyInstance.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT).AsDouble();
            Parameter paramSectionShape = familyInstance.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_SHAPE);

            result.Vzd = elemVz;

            double b_cr = 0.67 * parameterWidth;
            double S_bry = b_cr * Math.Pow(parameterHeight, 2) / 8;
            double I_y = b_cr * Math.Pow(parameterHeight, 3) / 12;
            double thau_d = elemVz * S_bry / (b_cr * I_y);

            result.Ratio3 = thau_d / result.fvd;

            ResultStatus status = new ResultStatus(ID);
            status.SetStatusRatioBased(result.Ratio1);
            manager.SetResult(result.GetEntity(), element, status);


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
