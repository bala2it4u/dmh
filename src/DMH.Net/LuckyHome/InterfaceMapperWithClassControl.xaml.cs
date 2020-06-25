namespace LuckyHome
{
    using EnvDTE;
    using LuckyHome.Common;
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public class InterfaceMapperWithClassControlInput
    {
        public ClassInfo[] ClassInfos
        {
            get;
            set;
        }

        public List<Project> ProjectNames
        {
            get;
            set;
        }

        public string ProjectNameSelected
        {
            get;
            set;
        }

        public Func<Project, List<CodeClass>> GetClasses
        {
            get;
            internal set;
        }

        public string[] ClassNames
        {
            get;
            set;
        }

        public string ClassNameSelected
        {
            get;
            set;
        }

        public Action<CodeClass[]> CallbackOption
        {
            get;
            internal set;
        }

        public Action Close
        {
            get;
            internal set;
        }

        public SchemaInfoCommon SchemaInfoCommon
        {
            get;
            internal set;
        }

        public Action ClearCache
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Interaction logic for InterfaceMapperWithClassControl.
    /// </summary>
    public partial class InterfaceMapperWithClassControl : UserControl
    {
        private InterfaceMapperWithClassControlInput input;
        private List<CodeClass> output = new List<CodeClass>();
        int index = 0;
        private List<CodeClass> classSource = new List<CodeClass>();

        public bool UseDefault;

        public InterfaceMapperWithClassControl()
        {
            InitializeComponent();
        }

        public void SetInterfaceMapperWithClassControlInput(InterfaceMapperWithClassControlInput input)
        {
            this.input = input;
            output = new List<CodeClass>();
            index = 0;
            cboProjectName.ItemsSource = input.ProjectNames.ConvertAll((Project x) => x.Name);
            SchemaInfo schemaInfo = input.SchemaInfoCommon.SchemaInfo;
            loadScreenData(schemaInfo);
            if (UseDefault && cboClassName.SelectedIndex != 0)
            {
                BtnUseDefault_Click(btnUseDefault, null);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (input != null)
            {
                UseDefault = false;
                input.ClassInfos[index].AssambleName = string.Concat(cboProjectName.SelectedValue);
                input.ClassInfos[index].NameSpaceAndMappedClassName = string.Concat(cboClassName.SelectedValue);
                CodeClass item = classSource.Find(delegate (CodeClass x)
                {
                    base.Dispatcher.VerifyAccess();
                    return x.FullName == string.Concat(cboClassName.SelectedValue);
                });
                output.Add(item);
                SchemaInfo schemaInfo = input.SchemaInfoCommon.SchemaInfo;
                ClassInfo classInfo = schemaInfo.DepandancyClasses.FirstOrDefault((ClassInfo x) => x.NameSpaceAndInterfaceName == lblInterfaceName.Text);
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

        private async void btnLastRun_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void BtnUseDefault_Click(object sender, RoutedEventArgs e)
        {
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
                    CodeClass tempClassdetail = input.GetClasses(project).FirstOrDefault((CodeClass x) => x.FullName == depandancyClasses.NameSpaceAndMappedClassName);
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
            int templastIndex = 0;
            int tempindex = 0;
            int TempSelectedIndex = cboClassName.SelectedIndex;
            if (interfaceName.LastIndexOf('.') != -1)
            {
                lblInterfaceName.Text = interfaceName;
                interfaceName = interfaceName.Substring(interfaceName.LastIndexOf('.') + 2);
                lblClassName.Text = interfaceName;
            }
            classSource.Any(delegate (CodeClass x)
            {
                ThreadHelper.ThrowIfNotOnUIThread("loadInterfaceClass");
                tempindex++;
                if (x.Name.EndsWith(interfaceName) && TempSelectedIndex < tempindex)
                {
                    templastIndex = tempindex;
                    return true;
                }
                return false;
            });
            cboClassName.SelectedIndex = templastIndex;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            input.Close();
        }

        private void CboProjectName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboProjectName.SelectedValue != null)
            {
                input.ProjectNameSelected = cboProjectName.SelectedValue.ToString();
                Project arg = input.ProjectNames.Find(delegate (Project x)
                {
                    base.Dispatcher.VerifyAccess();
                    return x.Name == input.ProjectNameSelected;
                });
                classSource = input.GetClasses(arg);
                int tempindex = 0;
                string tempIndefaceName = lblInterfaceName.Text;
                int templastIndex = 0;
                if (tempIndefaceName.LastIndexOf('.') != -1)
                {
                    tempIndefaceName = tempIndefaceName.Substring(tempIndefaceName.LastIndexOf('.') + 2);
                }
                List<string> list = classSource.ConvertAll(delegate (CodeClass x)
                {
                    ThreadHelper.ThrowIfNotOnUIThread("CboProjectName_SelectionChanged");
                    if (x.Name.EndsWith(tempIndefaceName))
                    {
                        templastIndex = tempindex + 1;
                    }
                    tempindex++;
                    return x.FullName;
                });
                list.Insert(0, "");
                cboClassName.ItemsSource = list;
                cboClassName.SelectedIndex = templastIndex;
            }
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            loadInterfaceClass(lblClassName.Text);
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            input.ClearCache();
            CboProjectName_SelectionChanged(sender, null);
        }

    }
}