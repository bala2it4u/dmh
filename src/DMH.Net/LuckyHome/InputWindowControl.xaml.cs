namespace LuckyHome
{
    //using EnvDTE;
    using LuckyHome.Common;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Web.Helpers;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for InputWindowControl.
    /// </summary>
    public partial class InputWindowControl : UserControl
    {
        ILogger logger = LogManager.GetLogger(typeof(InterfaceMapperWithClassControl));
        private InputWindowControlInput input;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputWindowControl"/> class.
        /// </summary>
        public InputWindowControl()
        {
            this.InitializeComponent();
        }

        internal void SetInterfaceMapperWithClassControlInput(InputWindowControlInput input)
        {
            logger.Info("SetInterfaceMapperWithClassControlInput created");
            this.input = input;
            cboProjectName.ItemsSource = Array.ConvertAll(input.SchemaInfo.StartAppProject,
                x => System.IO.Path.GetFileNameWithoutExtension(x));
            cboProjectName.SelectedIndex = 0;
            // input.ProjectNames.ConvertAll((x) => { Dispatcher.VerifyAccess(); return x.Name; });
            SchemaInfo schemaInfo = input.SchemaInfoCommon.SchemaInfo;
            loadScreenData(schemaInfo);
            
        }

        private void loadScreenData(SchemaInfo commonSchema)
        {
            logger.Info("loadScreenData called");
            //string tempIndefaceName = input.ClassInfos[index].NameSpaceAndInterfaceName;
            //lblInterfaceName.Text = tempIndefaceName;
            //ClassInfo classInfo = commonSchema.DepandancyClasses.FirstOrDefault((ClassInfo x) => x.NameSpaceAndInterfaceName == tempIndefaceName);
            var txt = new System.Windows.Documents.TextRange(txtInput.Document.ContentStart, txtInput.Document.ContentEnd);
            var tempData = input.LastRunFound != null ? 
                Json.Encode(input.LastRunFound.InputValues) : Json.Encode(input.SchemaInfo.InputValues);
            tempData = tempData.Replace("}", "}\n").Replace("\"DefaultValue","\n\"DefaultValue");
            txt.Text = tempData;
            //CboProjectName_SelectionChanged(cboProjectName, null);
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("ok_Click called");
            try
            {
                input.SchemaInfo.StartAppProject = new []{ input.SchemaInfo.StartAppProject[cboProjectName.SelectedIndex] };
                var inputValues = new System.Windows.Documents.TextRange(txtInput.Document.ContentStart, txtInput.Document.ContentEnd).Text;
                input.SchemaInfo.InputValues = Json.Decode<InputValue[]>(inputValues.Replace("\n",""));
                input.CallbackOption.Invoke();
            }
            catch (Exception ex)
            {
                logger.Error("ok_Click error", ex);
                    MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "please fix you json input data."),"error");
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            input.Close.Invoke();
        }
    }
}