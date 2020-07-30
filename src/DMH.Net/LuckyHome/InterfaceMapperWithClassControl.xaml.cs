namespace LuckyHome
{
    using EnvDTE;
    using LuckyHome.Common;
    using Microsoft.VisualStudio.Shell;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for InterfaceMapperWithClassControl.
    /// </summary>
    public partial class InterfaceMapperWithClassControl : UserControl
    {
        private IInterfaceMapperWithClassControlInput input;
        private List<CodeClass> output = new List<CodeClass>();
        int index = 0;
        private List<CodeClass> classSource = new List<CodeClass>();

        public bool UseDefault;
        ILogger logger = LogManager.GetLogger(typeof(InterfaceMapperWithClassControl));

        public InterfaceMapperWithClassControl()
        {
            InitializeComponent();
            logger.Info("InterfaceMapperWithClassControl created");
        }

        public void SetInterfaceMapperWithClassControlInput(IInterfaceMapperWithClassControlInput input)
        {
            logger.Info("SetInterfaceMapperWithClassControlInput created");
            this.input = input;
            output = new List<CodeClass>();
            index = 0;
            cboProjectName.ItemsSource = input.ProjectNames.ConvertAll((Project x) => { Dispatcher.VerifyAccess(); return x.Name; });
            SchemaInfo schemaInfo = input.SchemaInfoCommon.SchemaInfo;
            loadScreenData(schemaInfo);
            btnUselastrun.IsEnabled = input.LastRunFound;
            if (UseDefault && cboClassName.SelectedIndex != 0)
            {
                BtnUseDefault_Click(btnUseDefault, null);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("BtnNext_Click created");
            if (input != null)
            {
                UseDefault = false;
                input.ClassInfos[index].AssambleName = string.Concat(cboProjectName.SelectedValue);
                input.ClassInfos[index].NameSpaceAndMappedClassName = string.Concat(cboClassName.SelectedValue);
                CodeClass item = classSource.Find((CodeClass x) =>
                {
                    Dispatcher.VerifyAccess();
                    return x.FullName == string.Concat(cboClassName.SelectedValue);
                });
                output.Add(item);
                SchemaInfo schemaInfo = input.SchemaInfoCommon.SchemaInfo;
                ClassInfo classInfo = schemaInfo.DepandancyClasses.FirstOrDefault(x => x.NameSpaceAndInterfaceName == lblInterfaceName.Text);
                if (classInfo == null)
                {
                    classInfo = new ClassInfo();
                    schemaInfo.DepandancyClasses.Add(classInfo);
                }
                classInfo.AssambleName = input.ClassInfos[index].AssambleName;
                classInfo.NameSpaceAndMappedClassName = input.ClassInfos[index].NameSpaceAndMappedClassName;
                classInfo.NameSpaceAndInterfaceName = input.ClassInfos[index].NameSpaceAndInterfaceName;
                index++;
                if (input.ClassInfos.Length <= index)
                {
                    index = 0;
                    input.ClassInfos = new ClassInfo[0];
                    input.CallbackOption(output.ToArray());
                }
                else
                {
                    loadScreenData(schemaInfo);
                }
            }
        }

        private void loadScreenData(SchemaInfo commonSchema)
        {
            logger.Info("loadScreenData called");
            string tempIndefaceName = input.ClassInfos[index].NameSpaceAndInterfaceName;
            lblInterfaceName.Text = tempIndefaceName;
            ClassInfo classInfo = commonSchema.DepandancyClasses.FirstOrDefault((ClassInfo x) => x.NameSpaceAndInterfaceName == tempIndefaceName);
            if (classInfo != null)
            {
                cboProjectName.SelectedValue = classInfo.AssambleName;
                CboProjectName_SelectionChanged(cboProjectName, null);
                cboClassName.SelectedValue = classInfo.NameSpaceAndMappedClassName;
                lblClassName.Text = classInfo.NameSpaceAndMappedClassName;
            }
            else
            {
                loadInterfaceClass(tempIndefaceName);
            }
        }

        private void BtnUseDefault_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("BtnUseDefault_Click clicked");
            if (input == null || !input.ClassInfos.Any())
            {
                return;
            }
            BtnNext_Click(sender, e);
            UseDefault = true;
            SchemaInfo commonSchema = input.SchemaInfoCommon.SchemaInfo;
            string tempIndefaceName = input.ClassInfos[index].NameSpaceAndInterfaceName;
            while (true)
            {
                ClassInfo depandancyClasses = commonSchema.DepandancyClasses.FirstOrDefault((ClassInfo x) => x.NameSpaceAndInterfaceName == tempIndefaceName);
                if (depandancyClasses == null)
                {
                    break;
                }
                input.ClassInfos[index].AssambleName = depandancyClasses.AssambleName;
                input.ClassInfos[index].NameSpaceAndMappedClassName = depandancyClasses.NameSpaceAndMappedClassName;
                Project project = input.ProjectNames.Find((Project x) => x.Name == depandancyClasses.AssambleName);
                if (depandancyClasses.NameSpaceAndMappedClassName != "")
                {
                    CodeClass tempClassdetail = input.GetClasses(project).FirstOrDefault(
                        (CodeClass x) => x.FullName == depandancyClasses.NameSpaceAndMappedClassName);
                    if (tempClassdetail == null)
                    {
                        loadInterfaceClass(tempIndefaceName);
                        return;
                    }
                    output.Add(tempClassdetail);
                }
                else
                {
                    output.Add(null);
                }
                index++;
                if (input.ClassInfos.Length <= index)
                {
                    index = 0;
                    input.ClassInfos = new ClassInfo[0];
                    input.CallbackOption(output.ToArray());
                    return;
                }
                tempIndefaceName = input.ClassInfos[index].NameSpaceAndInterfaceName;
            }
            loadInterfaceClass(tempIndefaceName);
        }

        private void loadInterfaceClass(string interfaceName)
        {
            logger.Info("loadInterfaceClass called");
            int templastIndex = 0;
            int tempindex = 0;
            int tempSelectedIndex = cboClassName.SelectedIndex;
            if (interfaceName.LastIndexOf('.') != -1)
            {
                lblInterfaceName.Text = interfaceName;
                interfaceName = interfaceName.Substring(interfaceName.LastIndexOf('.') + 2);
                lblClassName.Text = interfaceName;
            }
            var tempclassSource = cboClassName.ItemsSource as List<string>;
            if (tempclassSource != null)
            {
                tempclassSource.Any((string fullname) =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread("loadInterfaceClass");
                    tempindex++;
                    if (fullname.EndsWith(interfaceName) && tempSelectedIndex < tempindex)
                    {
                        templastIndex = tempindex - 1;
                        return true;
                    }
                    return false;
                });
                cboClassName.SelectedIndex = templastIndex;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("BtnCancel_Click clicked");
            input.Close();
        }

        private void CboProjectName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logger.Info("CboProjectName_SelectionChanged clicked");
            if (cboProjectName.SelectedValue != null)
            {
                input.ProjectNameSelected = cboProjectName.SelectedValue.ToString();
                Project arg = input.ProjectNames.Find((Project x) =>
                {
                    base.Dispatcher.VerifyAccess();
                    return x.Name == input.ProjectNameSelected;
                });
                classSource = input.GetClasses(arg);
                //var classSource1 = input.GetRefClasses(arg);
                int tempindex = 0;
                string tempIndefaceName = lblInterfaceName.Text;
                int templastIndex = 0;
                if (tempIndefaceName.LastIndexOf('.') != -1)
                {
                    tempIndefaceName = tempIndefaceName.Substring(tempIndefaceName.LastIndexOf('.') + 2);
                }
                List<string> list = classSource.ConvertAll((CodeClass x) =>
                {
                    Dispatcher.VerifyAccess();
                    if (x.Name.EndsWith(tempIndefaceName))
                    {
                        templastIndex = tempindex + 1;
                    }
                    tempindex++;
                    string text = x.Namespace.Name + ".";
                    return text + x.FullName.Replace(text, "").Replace('.', '+');

                });
                list.Insert(0, "");
                //list.AddRange(classSource1);
                cboClassName.ItemsSource = list;
                cboClassName.SelectedIndex = templastIndex;
            }
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("BtnFind_Click clicked");
            loadInterfaceClass(lblClassName.Text);
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("BtnReload_Click clicked");
            input.ClearCache();
            CboProjectName_SelectionChanged(sender, null);
        }

        private void btnLastRun_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("btnLastRun_Click clicked");
            if (input.LastRunFound)
            {
                input.CallbackLastRunOption();
            }
        }

        private void BtnSkip_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("BtnSkip_Click clicked");
            while (true)
            {
                input.ClassInfos[index].NameSpaceAndMappedClassName = "";
                index++;
                if (input.ClassInfos.Length <= index)
                {
                    index = 0;
                    input.ClassInfos = new ClassInfo[0];
                    input.CallbackOption(output.ToArray());
                    break;
                }
            }
        }
    }
}