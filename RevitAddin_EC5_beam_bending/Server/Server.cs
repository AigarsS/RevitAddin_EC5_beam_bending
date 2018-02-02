using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

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
            Autodesk.Revit.DB.CodeChecking.Storage.StorageService service = Autodesk.Revit.DB.CodeChecking.Storage.StorageService.GetStorageService();
            Autodesk.Revit.DB.CodeChecking.Storage.StorageDocument storageDocument = service.GetStorageDocument(data.Document);


            foreach (Element element in data.Selection)
            {
                Autodesk.Revit.DB.CodeChecking.Storage.Label ccLabel = storageDocument.LabelsManager.GetLabel(element);

                if (ccLabel != null)
                {
                    Label myLabel = ccLabel.GetEntity<Label>(data.Document);

                    if (myLabel != null)
                    {
                        CalculationParameter myParams = storageDocument.CalculationParamsManager.CalculationParams.GetEntity<CalculationParameter>(data.Document);

                        Result myResult = new Result();

                        storageDocument.ResultsManager.SetResult(myResult.GetEntity(), element);
                    }
                }
            }
        }

        public override IList<BuiltInCategory> GetSupportedCategories(StructuralAssetClass material)
        {
            return new List<BuiltInCategory>() { BuiltInCategory.OST_ColumnAnalytical, BuiltInCategory.OST_BeamAnalytical, BuiltInCategory.OST_WallAnalytical, BuiltInCategory.OST_FloorAnalytical, BuiltInCategory.OST_WallFoundationAnalytical, BuiltInCategory.OST_IsolatedFoundationAnalytical };
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
