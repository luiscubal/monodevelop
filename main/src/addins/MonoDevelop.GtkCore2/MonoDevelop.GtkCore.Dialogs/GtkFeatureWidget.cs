
using System;
using MonoDevelop.Ide.Templates;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using Gtk;

namespace MonoDevelop.GtkCore.Dialogs
{
	class GtkFeatureWidget : Gtk.VBox
	{
		ComboBox versionCombo;
		
		public GtkFeatureWidget (DotNetProject project)
		{
			Spacing = 6;
			
			versionCombo = Gtk.ComboBox.NewText ();
			ReferenceManager refmgr = new ReferenceManager (project);
			foreach (string v in refmgr.SupportedGtkVersions)
				versionCombo.AppendText (v);
			versionCombo.Active = 0;
			refmgr.Dispose ();
			
			// GTK# version selector
			HBox box = new HBox (false, 6);
			Gtk.Label vlab = new Label (GettextCatalog.GetString ("Target GTK# version:"));
			box.PackStart (vlab, false, false, 0);
			box.PackStart (versionCombo, false, false, 0);
			box.PackStart (new Label (GettextCatalog.GetString ("(or upper)")), false, false, 0);
			PackStart (box, false, false, 0);
			
			ShowAll ();
		}
		
		public string SelectedVersion {
			get { return versionCombo.ActiveText; }
		}
	}
	
	class GtkProjectFeature: ISolutionItemFeature
	{
		public string Title {
			get { return GettextCatalog.GetString ("GTK# Support"); }
		}
		
		public string Description {
			get { return GettextCatalog.GetString ("Enables support for GTK# in the project. Allows the visual design of GTK# windows, and the creation of a GTK# widget library."); }
		}

		public FeatureSupportLevel GetSupportLevel (SolutionFolder parentCombine, SolutionItem entry)
		{
			if (!(entry is DotNetProject) || !GtkDesignInfo.SupportsRefactoring (entry as DotNetProject))
				return FeatureSupportLevel.NotSupported;
			else if (GtkDesignInfo.SupportsDesigner ((Project)entry))
				return FeatureSupportLevel.Enabled;
			else if (entry is DotNetAssemblyProject)
				return FeatureSupportLevel.SupportedByDefault;
			else
				return FeatureSupportLevel.Supported;
		}
		
		public Widget CreateFeatureEditor (SolutionFolder parentCombine, SolutionItem entry)
		{
			return new GtkFeatureWidget ((DotNetProject) entry);
		}

		public void ApplyFeature (SolutionFolder parentCombine, SolutionItem entry, Widget editor)
		{
			GtkFeatureWidget fw = (GtkFeatureWidget) editor;
			ReferenceManager refmgr = new ReferenceManager ((DotNetProject) entry);
			refmgr.GtkPackageVersion = fw.SelectedVersion;
			refmgr.Dispose ();
		}
		
		public string Validate (SolutionFolder parentCombine, SolutionItem entry, Gtk.Widget editor)
		{
			return null;
		}
	}
}
