using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        private Form2 form2;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 单击按钮，弹出对话框，选择地图文件夹
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择地图文件夹";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                // 获取选择的路径
                Commons.MapPath = dialog.SelectedPath;
                // 先判断路径是否正确
                if (!Commons.CheckMapPath(Commons.MapPath))
                {
                    // 触发点击事件，弹出选择路径对话框
                    this.button1.PerformClick();
                }
                Commons.GamePath = Directory.GetParent(Commons.MapPath).FullName;
                Commons.GameFile = Commons.GamePath + "\\war3.exe";
                this.label1.Text = Commons.MapPath;
                // 写入配置文件 section1 - MapPath
                Commons.Write("section1", "MapPath", Commons.MapPath, Commons.ConfigPath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 初始化config.ini
            if (!File.Exists(Commons.ConfigPath))
            {
                // 创建config.ini文件
                FileStream ConfigFS = File.Create(Commons.ConfigPath);
                ConfigFS.Close();
            }
            else
            {
                // config文件存在
                // 读取配置文件 section1 - MapPath
                
                Commons.MapPath = Commons.Read("section1", "MapPath", "", Commons.ConfigPath);
                // 先判断路径是否正确
                if (!Commons.CheckMapPath(Commons.MapPath))
                {
                    // 触发点击事件，弹出选择路径对话框
                    this.button1.PerformClick();
                }
                Commons.GamePath = Directory.GetParent(Commons.MapPath).FullName;
                Commons.GameFile = Commons.GamePath + "\\war3.exe";
                // 设置label1 Text
                this.label1.Text = Commons.MapPath;
            }

            // 初始化ListView1
            this.listView1.Columns.Add("文件名", 100);
            this.listView1.Columns.Add("地图名", 100);
            this.listView1.Columns.Add("大小", 100);
            this.listView1.Columns.Add("修改时间", 100);
            

        
        }

        private void label1_TextChanged(object sender, EventArgs e)
        {
            // Text初始值为空字符串，当路径发生变化时【赋值、修改】，处理数据

            // 处理TreeView1
            // 设置根节点
            TreeNode RootNode = new TreeNode() { Tag = Commons.MapPath, Text = "Maps" };
            this.treeView1.Nodes.Add(RootNode);

            // 加载当前文件夹
            foreach(var dir in Directory.GetDirectories(Commons.MapPath))
            {
                TreeNode node = new TreeNode() { Tag = dir , Text = Path.GetFileName(dir) };
                //node.Nodes.Add("");
                // 递归将子文件夹添加到父文件夹中
                node = Commons.ListDirectories(node);
                this.treeView1.Nodes[0].Nodes.Add(node);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                // 保存选择的TreeNode

                // 清除原内容
                this.listView1.Items.Clear();
                // 获取当前目录下的文件，显示到listView1
                foreach (var file in Directory.GetFiles(e.Node.Tag.ToString(), "*.w3?", SearchOption.TopDirectoryOnly))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // 文件名 【地图名】 大小 修改时间
                    // 处理地图名
                    string[] fileInfoUsed = { 
                        fileInfo.Name,
                        Commons.GetWar3MapName(file),
                        (fileInfo.Length / 1024 + 1) + "KB",
                        fileInfo.LastWriteTime.ToString()};
                    ListViewItem item = new ListViewItem(fileInfoUsed) { Tag = fileInfo.FullName};
                    // 设置图标索引
                    item.ImageIndex = 1;
                    this.listView1.Items.Add(item);               
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 使用War3打开地图
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 文件删除
            // 选择项数量非0
            if(this.listView1.SelectedItems.Count != 0)
            {
                // 遍历选择项
                foreach(ListViewItem item in this.listView1.SelectedItems)
                {
                    if(File.Exists(item.Tag.ToString()))
                    {
                        // 确认删除
                        if(MessageBox.Show("是否删除文件？", "删除", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            File.Delete(item.Tag.ToString());
                            // 删除listView1中的项
                            this.listView1.Items.RemoveAt(item.Index);
                        }
                    }
                    else
                    {
                        MessageBox.Show("文件不存在!");
                    }
                }
            }
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 文件重命名
            // 选择项数量1
            if (this.listView1.SelectedItems.Count == 1)
            {
                // 遍历选择项
                foreach (ListViewItem item in this.listView1.SelectedItems)
                {
                    if (File.Exists(item.Tag.ToString()))
                    {
                        // 
                        
                    }
                    else
                    {
                        MessageBox.Show("文件不存在!");
                    }
                }
            }
        }

        public void StartGame1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                if (this.listView1.SelectedItems.Count == 1)
                {
                    // 获取选择文件的全路径
                    string Map = this.listView1.SelectedItems[0].Tag.ToString();
                    StartGame.StartGame1(Map);
                }
            
        }

        public void StartGame2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //StartGame.StartGame2();
        }

        public void StartGame3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                // 获取选择文件的文件名
                string Map = this.listView1.SelectedItems[0].Tag.ToString();
                // 截取字符串
                Map = Map.Substring(Map.IndexOf("Maps\\"));
                StartGame.StartGame3(Map);
            }
        }

        private void FindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 查找
            // 弹出窗体Form2
            // 通过单例模式创建Form2
            this.form2 = Form2.CreateInstance(this);
            form2.Show();
            form2.BringToFront(); // 显示到最顶层
            form2.Focus();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // 判断被哪一个控件打开的
            //string name = (sender as ContextMenuStrip).SourceControl.Name;
        }

        private void BookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 收藏
            if (this.listView1.SelectedItems.Count != 0)
            {
                foreach (ListViewItem item in this.listView1.SelectedItems)
                {
                    // 不在收藏中
                    if (!Commons.BookmarkMapItem.Contains(item))
                    {
                        // 添加到收藏
                        Commons.BookmarkMapItem.Add(item);
                    }                   
                }
            }
        }

        private void UnBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 取消收藏
            if (this.listView1.SelectedItems.Count != 0)
            {
                foreach (ListViewItem item in this.listView1.SelectedItems)
                {
                    // 在收藏中
                    if (!Commons.BookmarkMapItem.Contains(item))
                    {
                        // 删除收藏
                        Commons.BookmarkMapItem.Remove(item);
                    }
                }
            }
        }
    }
}
