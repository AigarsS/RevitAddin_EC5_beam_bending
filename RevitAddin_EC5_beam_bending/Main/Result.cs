using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage.Framework.Documentation.Attributes;

namespace RevitAddin_EC5_beam_bending
{
    [Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes.Schema("Result", "bbe5870b-bb46-4bfd-a272-ec43beaf395a")]
    public class Result : Autodesk.Revit.DB.ExtensibleStorage.Framework.SchemaClass
    {
        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Force, DisplayUnit = DisplayUnitType.DUT_NEWTONS)]
        [ValueWithName(Name = "N{pl,Rd}", Index = 5)]
        public Double Nplrd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Force, DisplayUnit = DisplayUnitType.DUT_NEWTONS)]
        [ValueWithName(Name = "N{u,Rd}", Index = 6)]
        public Double Nurd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Force, DisplayUnit = DisplayUnitType.DUT_NEWTONS)]
        [ValueWithName(Name = "N{Rd}", Index = 7)]
        public Double Nrd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Section_Area, DisplayUnit = DisplayUnitType.DUT_SQUARE_METERS)]
        [ValueWithName(Name = "A{net}", Index = 2)]
        public Double Anet { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Section_Area, DisplayUnit = DisplayUnitType.DUT_SQUARE_METERS)]
        [ValueWithName(Index = 0)]
        public Double A { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "f{m,k}", Index = 3)]
        public Double fmk { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "f{c,0,k}", Index = 4)]
        public Double fc0k { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Number, DisplayUnit = DisplayUnitType.DUT_1_RATIO)]
        [Ratio(Index = 8)]
        public Double Ratio { get; set; }

        public Result()
        {
        }

        public Result(Document document)
        {
        }

        public Result(Autodesk.Revit.DB.ExtensibleStorage.Entity entity, Document document)
            : base(entity, document)
        {
        }
    }
}

