using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Windows.Forms;
using PSPatcher.Core.AstExtensions.CSharp;
using PSPatcher.Core.AstExtensions.PS;
using PSPatcher.Core.Parsers;
using PSPatcher.Core.Storage;

namespace PSPatcher
{
    public partial class Form1 : Form
    {
        private readonly FileStorage fileStorage;
        private readonly StringStorage stringStorage;
        private PSScriptAst psScriptAst;

        public Form1()
        {
            fileStorage = new FileStorage();
            stringStorage = new StringStorage();
            
            InitializeComponent();
        }

        private void menuOpen_Click(object sender, System.EventArgs e)
        {
            openPSFileDialog.FileName = "TestingChallenge.ps1";
            openPSFileDialog.Filter = "powershell script|*.ps1";
            openPSFileDialog.FilterIndex = 0;
            openPSFileDialog.RestoreDirectory = true;

            if (openPSFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    psScriptAst = fileStorage.Open(openPSFileDialog.FileName);
                    var psParseResult = PSFileParser.Parse(psScriptAst);

                    RefreshWindow(psParseResult);

                    fileStatusLabel.Text = openPSFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error: Could not open file. Original error: {0}", ex.Message));
                }
            }
        }

        private void menuSave_Click(object sender, System.EventArgs e)
        {
            savePSFileDialog.FileName = "ResultScript";
            savePSFileDialog.DefaultExt = ".ps1"; 
            savePSFileDialog.Filter = "powershell script|*.ps1";
            savePSFileDialog.RestoreDirectory = true;

            if (savePSFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fileStorage.Save(rawCodeTextBox.Text, savePSFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error: Could not open file. Original error: {0}", ex.Message));
                }
            }
        }

        private void menuInvoke_Click(object sender, System.EventArgs e)
        {
            if (psScriptAst != null)
            {
                try
                {
                    resultsTextBox.Text += psScriptAst.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error: Could not invoke script. Original error: {0}", ex.Message));
                }
            }
            else
            {
                MessageBox.Show("Error: no file");
            }
        }

        private void RefreshWindow(PSParseResult psParseResult)
        {
            SetScriptText(psScriptAst.GetText());
            
            SetClassNames(psParseResult.CSharpClasses);

            SetInvokedMethods(psParseResult.InvokeMembers);

            SetClassesMethods(psParseResult.CSharpClasses);

            SetTree(psParseResult);
        }

        private void SetClassesMethods(List<CSharpClassAst> cSharpClasses)
        {
            classesMethodsComboBox.BeginUpdate();
            classesMethodsComboBox.Items.Clear();

            classesMethodsComboBox.Items.AddRange(cSharpClasses.SelectMany(x => x.GetMethodsName()).ToArray());

            if (classesMethodsComboBox.Items.Count > 0)
                classesMethodsComboBox.SelectedIndex = 0;

            classesMethodsComboBox.EndUpdate();
        }

        private void SetInvokedMethods(List<InvokeMember> invokeMembers)
        {
            invokedMembersComboBox.BeginUpdate();
            invokedMembersComboBox.Items.Clear();

            invokedMembersComboBox.Items.AddRange(invokeMembers.ToArray());

            if (invokedMembersComboBox.Items.Count > 0)
                invokedMembersComboBox.SelectedIndex = 0;

            invokedMembersComboBox.EndUpdate();
        }

        private void SetTree(PSParseResult psParseResult)
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            AddPSNode(psScriptAst);

            AddCSharpClassesNode(psParseResult.CSharpClasses);

            AddInvokeMembersNode(psParseResult.InvokeMembers);

            treeView.EndUpdate();
        }

        private void AddInvokeMembersNode(List<InvokeMember> invokeMembers)
        {
            var classesNode = new TreeNode("Invoke Members");
            treeView.Nodes.Add(classesNode);

            foreach (var invokeMember in invokeMembers)
            {
                var node = new TreeNode(invokeMember.MethodName);
                node.ToolTipText = invokeMember.Ast.ToString();
                classesNode.Nodes.Add(node);
            }
        }

        private void AddCSharpClassesNode(List<CSharpClassAst> csharpClasses)
        {
            var classesNode = new TreeNode("C# classes");
            treeView.Nodes.Add(classesNode);

            foreach (var csharpClass in csharpClasses)
            {
                var node = new TreeNode(csharpClass.GetClassName());
                node.ToolTipText = csharpClass.GetText();
                classesNode.Nodes.Add(node);
                node.Nodes.AddRange(csharpClass.GetTree());
            }
        }

        private void AddPSNode(PSScriptAst psAst)
        {
            var node = new TreeNode("PowerShell");
            treeView.Nodes.Add(node);
            node.Nodes.AddRange(psAst.GetTree());
        }

        private void SetClassNames(List<CSharpClassAst> csharpClasses)
        {
            classeNamesComboBox.BeginUpdate();
            classeNamesComboBox.Items.Clear();

            classeNamesComboBox.Items.AddRange(csharpClasses.ToArray());

            if (classeNamesComboBox.Items.Count > 0)
                classeNamesComboBox.SelectedIndex = 0;

            classeNamesComboBox.EndUpdate();
        }

        private void SetScriptText(string rawText)
        {
            rawCodeTextBox.Text = rawText;
        }

        private void menuAddMethodMulToClass_Click(object sender, EventArgs e)
        {
            var sharpClassAst = (CSharpClassAst) classeNamesComboBox.SelectedItem;

            sharpClassAst.AddMethod("public int Mul(int a, int b){return a* b;}");
            psScriptAst.PatchCSharpCode(sharpClassAst.GetText(), sharpClassAst.Ast);

            psScriptAst = stringStorage.Open(psScriptAst.GetText());
            var psParseResult = PSFileParser.Parse(psScriptAst);

            RefreshWindow(psParseResult);
        }

        private void menuChangeCallingMethod_Click(object sender, EventArgs e)
        {
            var invokeMember = (InvokeMember) invokedMembersComboBox.SelectedItem;
            var newMethodName = (string) classesMethodsComboBox.SelectedItem;

            psScriptAst.PatchInvokeMethod(newMethodName, invokeMember.Ast);

            psScriptAst = stringStorage.Open(psScriptAst.GetText());
            var psParseResult = PSFileParser.Parse(psScriptAst);

            RefreshWindow(psParseResult);
        }
    }
}
