using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitAddin_EC5_beam_bending
{
    public class RevitApplicationDB : Autodesk.Revit.DB.IExternalDBApplication
    {
        #region IExternalDBApplication Members

        public Autodesk.Revit.DB.ExternalDBApplicationResult OnShutdown(Autodesk.Revit.ApplicationServices.ControlledApplication application)
        {
            return Autodesk.Revit.DB.ExternalDBApplicationResult.Succeeded;
        }

        public Autodesk.Revit.DB.ExternalDBApplicationResult OnStartup(Autodesk.Revit.ApplicationServices.ControlledApplication application)
        {
            application.ApplicationInitialized += new EventHandler<Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs>(application_ApplicationInitialized);


            return Autodesk.Revit.DB.ExternalDBApplicationResult.Succeeded;
        }

        void application_ApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
        {
            Server server = new Server();

            Autodesk.Revit.DB.CodeChecking.Service.AddServer(server);

            Updater updater = new Updater();
            UpdaterRegistry.RegisterUpdater(updater);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementClassFilter(typeof(FamilyInstance)), Element.GetChangeTypeGeometry());
        }

        #endregion
    }
}
