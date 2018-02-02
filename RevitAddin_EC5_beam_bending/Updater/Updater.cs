using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitAddin_EC5_beam_bending
{
    /// <summary>
    /// Represents updater which main task is to invalidate results when any geometry change is done.
    /// </summary>
    class Updater : IUpdater
    {
        #region IUpdater Members

        public static Guid UpdaterId = new Guid("ad2afcdf-2b55-413a-92e4-8424cabeb0e8");
        public static Guid AddinId = new Guid("d6492ce6-de3c-47d2-b8da-f7e5d614379e");

        public Updater()
        {
        }

        public void Execute(UpdaterData data)
        {
            Autodesk.Revit.DB.CodeChecking.Storage.StorageService service = Autodesk.Revit.DB.CodeChecking.Storage.StorageService.GetStorageService();
            Autodesk.Revit.DB.CodeChecking.Storage.StorageDocument storageDocument = service.GetStorageDocument(data.GetDocument());

            foreach (ElementId elId in data.GetModifiedElementIds())
            {
                Element el = data.GetDocument().GetElement(elId);
                storageDocument.ResultsManager.OutDateResults(Server.ID, el);
            }
        }

        public string GetAdditionalInformation()
        {
            return "";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Structure;
        }

        public UpdaterId GetUpdaterId()
        {
            return new UpdaterId(new AddInId(AddinId), UpdaterId);
        }

        public string GetUpdaterName()
        {
            return "Code Checking";
        }

        #endregion
    }
}