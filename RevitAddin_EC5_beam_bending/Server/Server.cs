using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.CodeChecking.Storage;
using Autodesk.Revit.DB.CodeChecking;
using Autodesk.Revit.DB.Structure;

namespace RevitAddin_EC5_beam_bending
{
    [Autodesk.Revit.DB.CodeChecking.Attributes.ServerVersion(1)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.CalculationParamsStructure(typeof(CalculationParameter))]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_ColumnAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.ResultStructure(typeof(Result), BuiltInCategory.OST_ColumnAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_BeamAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.ResultStructure(typeof(Result), BuiltInCategory.OST_BeamAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_WallAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.ResultStructure(typeof(Result), BuiltInCategory.OST_WallAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_FloorAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.ResultStructure(typeof(Result), BuiltInCategory.OST_FloorAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_WallFoundationAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.ResultStructure(typeof(Result), BuiltInCategory.OST_WallFoundationAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_IsolatedFoundationAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.ResultStructure(typeof(Result), BuiltInCategory.OST_IsolatedFoundationAnalytical, StructuralAssetClass.Basic)]
    public class Server : Autodesk.Revit.DB.CodeChecking.Documentation.MultiStructureServer
    {
        public static readonly Guid ID = new Guid("8e273cf2-f9a0-45b5-9e83-b1a0068d56b5");
        #region ICodeCheckingServer Members

        public override void Verify(Autodesk.Revit.DB.CodeChecking.ServiceData data)
        {
            StorageService service = StorageService.GetStorageService();
            StorageDocument storageDocument = service.GetStorageDocument(data.Document);
            CalculationParameter myParams = storageDocument.CalculationParamsManager.CalculationParams.GetEntity<CalculationParameter>(data.Document);
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
                        Calculate(myParams, myLabel, element, storageDocument.ResultsManager);
                    }
                }
            }
        }

        void Calculate(CalculationParameter parameters, Label label, Element element, ResultsManager manager)
        {
            Result result = new Result();
            Material mat = Tools.GetMaterialOfElement(element);
            PropertySetElement propertySetElement = mat.Document.GetElement(mat.StructuralAssetId) as PropertySetElement;
            StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();
            result.fmk = UnitUtils.ConvertFromInternalUnits(structuralAsset.WoodBendingStrength, DisplayUnitType.DUT_PASCALS);
            result.fc0k = UnitUtils.ConvertFromInternalUnits(structuralAsset.WoodParallelCompressionStrength, DisplayUnitType.DUT_PASCALS);

            AnalyticalModel analyticalModel = element as AnalyticalModel;
            FamilyInstance familyInstance = element.Document.GetElement(analyticalModel.GetElementId()) as FamilyInstance;
            Parameter parameterA = familyInstance.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_AREA);

            //if (parameterA != null)
            //{
            //    result.A = UnitUtils.ConvertFromInternalUnits(parameterA.AsDouble(), DisplayUnitType.DUT_SQUARE_METERS);
            //    result.Anet = result.A; //- label.Aholes;
            //    result.Nplrd = result.A * result.fmk / parameters.gammaM;
            //    result.Nurd = 0.9 * result.Anet * result.fc0k / parameters.PartialFactor2;
            //    result.Nrd = Math.Min(result.Nplrd, result.Nurd);
            //    result.Ratio = 1; //label.N / result.Nrd;
            //    ResultStatus status = new ResultStatus(ID);
            //    status.SetStatusRatioBased(result.Ratio);
            //    manager.SetResult(result.GetEntity(), element, status);
            //}
        }

        public override IList<BuiltInCategory> GetSupportedCategories(StructuralAssetClass material)
        {
            return new List<BuiltInCategory>() { BuiltInCategory.OST_BeamAnalytical};
        }

        public override IList<Autodesk.Revit.DB.StructuralAssetClass> GetSupportedMaterials()
        {
            return new List<Autodesk.Revit.DB.StructuralAssetClass>() { StructuralAssetClass.Basic };
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
