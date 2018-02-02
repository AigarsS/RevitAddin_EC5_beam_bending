using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitAddin_EC5_beam_bending
{
    [Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes.Schema("Result", "bbe5870b-bb46-4bfd-a272-ec43beaf395a")]
    public class Result : Autodesk.Revit.DB.ExtensibleStorage.Framework.SchemaClass
    {
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

