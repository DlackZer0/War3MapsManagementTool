using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form2 : Form
    {
        private Form1 form1;
        private static Form2 instance;
        public Form2()
        {
            InitializeComponent();
        }
        public static Form2 CreateInstance(Form1 form1)
        {
            
            if (instance == null)
            {
                instance = new Form2();
            }
            // 判断窗口是否被销毁
            if (instance.IsDisposed)
            {
                instance = new Form2();
            }
            // 获取父窗口对象
            instance.form1 = form1;
            return instance;
        }

        // 在第一次显示窗体前发生
        private void Form2_Load(object sender, EventArgs e)
        {
            // 初始化listview1
            this.listView1.Columns.Add("文件名", 100);
            this.listView1.Columns.Add("地图名", 100);
            this.listView1.Columns.Add("大小", 100);
            this.listView1.Columns.Add("修改时间", 100);

            // 小图标
            this.listView1.SmallImageList = form1.imageList1;
            // 右键
            this.listView1.ContextMenuStrip = Form1.contextMenuStrip1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 点击查找
            int idx;
            ListView listViewFinded = null;
            // 判断输入框是否为空
            if (this.comboBox1.Text != "")
            {
                // 获取输入的内容
                string i = comboBox1.Text;

                // 判断查找条件
                if (radioButton1.Checked)
                {
                    // 按文件名查找
                    idx = 0;
                }
                else
                {
                    // 按地图名查找
                    idx = 1;
                }
                if (radioButton3.Checked)
                {
                    // 当前文件夹
                    listViewFinded = this.form1.listView1;
                }
                else
                {
                    // 全部文件夹

                    // 获取全部地图
                    if (Commons.AllMapListView == null)
                    {
                        Commons.AllMapListView = new ListView();
                        try
                        {
                            foreach (var file in Directory.GetFiles(Commons.MapPath, "*.w3?", SearchOption.AllDirectories))
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                // 文件名 【地图名】 大小 修改时间
                                // 处理地图名
                                string[] fileInfoUsed = {
                        fileInfo.Name,
                        Commons.GetWar3MapName(file),
                        (fileInfo.Length / 1024 + 1) + "KB",
                        fileInfo.LastWriteTime.ToString()};
                                ListViewItem item = new ListViewItem(fileInfoUsed) { Tag = fileInfo.FullName };
                                // 设置图标索引
                                item.ImageIndex = 1;
                                Commons.AllMapListView.Items.Add(item);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    
                    listViewFinded = Commons.AllMapListView;
                }

                // 查找文件
                // 清除原内容
                this.listView1.Items.Clear();
                List<ListViewItem> list = Commons.FindFile(idx, listViewFinded, i);
                // 将找到的文件项的克隆添加到form2.listview1
                foreach (ListViewItem item in list)
                {
                    this.listView1.Items.Add((ListViewItem)item.Clone());
                }
            }

        }
    }
}
