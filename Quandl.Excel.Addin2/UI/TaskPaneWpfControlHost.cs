using Quandl.Shared;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace Quandl.Excel.Addin.UI
{
    [ComVisible(true)]
    public partial class TaskPaneWpfControlHost : UserControl
    {
        private readonly System.Windows.Controls.UserControl wpfControl;

        protected System.Windows.Controls.UserControl WpfControl
        {
            get { return wpfControl; }
        }
        public TaskPaneWpfControlHost()
        {
            InitializeComponent();

            
        }

        protected TaskPaneWpfControlHost(System.Windows.Controls.UserControl control)
        : this()
        {
            this.wpfControl = control;
            WpfElementHost.HostContainer.Children.Add(wpfControl);
        }
        

        public ElementHost WpfElementHost { get; private set; }
    }
    [ComVisible(true)]
    [ProgId("QuandlExcelAddin2.AboutControlHost")]
    public class AboutControlHost : TaskPaneWpfControlHost
    {
        public AboutControlHost()
            : base(new About())
        {

        }
    }

    [ComVisible(true)]
    [ProgId("QuandlExcelAddin2.SettingsControlHost")]
    public class SettingsControlHost : TaskPaneWpfControlHost
    {
        public SettingsControlHost()
            : base(new Settings.Settings())
        {

        }

        public void Reset()
        {
            ((Settings.Settings) this.WpfControl).Reset();
        }
    }

    [ComVisible(true)]
    [ProgId("QuandlExcelAddin2.WizardGuideControlHost")]
    public class WizardGuideControlHost : TaskPaneWpfControlHost
    {
        public WizardGuideControlHost()
            : base(new UDF_Builder.WizardGuide())
        {
            this.Reset();
        }

        public void Reset()
        {
            var guide = (UDF_Builder.WizardGuide)this.WpfControl;
            guide.Reset();
            guide.Background = Brushes.White;
            guide.Margin = new Thickness(0);
            guide.Padding = new Thickness(0);

        }
    }

    [ComVisible(true)]
    [ProgId("QuandlExcelAddin2.UpdateControlHost")]
    public class UpdateControlHost : TaskPaneWpfControlHost
    {
        public UpdateControlHost()
            : base(new Update(()=>MainLogic.Instance.Updater))
        {

        }

        public void UpdateContent()
        {
            ((Update) this.WpfControl).UpdateContent();
        }
    }
}