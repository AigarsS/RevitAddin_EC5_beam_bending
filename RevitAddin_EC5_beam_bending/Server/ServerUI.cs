using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitAddin_EC5_beam_bending
{
    [Autodesk.Revit.DB.CodeChecking.Attributes.CalculationParamsStructure(typeof(CalculationParameter))]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_ColumnAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_BeamAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_WallAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_FloorAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_WallFoundationAnalytical, StructuralAssetClass.Basic)]
    [Autodesk.Revit.DB.CodeChecking.Attributes.LabelStructure(typeof(Label), BuiltInCategory.OST_IsolatedFoundationAnalytical, StructuralAssetClass.Basic)]
    public class ServerUI : Autodesk.Revit.UI.CodeChecking.MultiStructureServer
    {
        #region ICodeCheckingServerUI Members

        public override string GetResource(string key, string context)
        {
            string txt = RevitAddin_EC5_beam_bending.Properties.Resources.ResourceManager.GetString(key);

            if (!string.IsNullOrEmpty(txt))
                return txt;

            return key;
        }

        public override Uri GetResourceImage(string key, string context)
        {
            return new Uri(@"pack://application:,,,/Code_Checking;component/" + key);
        }

        #endregion
    }
}
