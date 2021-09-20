/****李辉 1652286****/
/**2020.07.20**/

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Huffman
{
    public partial class Huffman : Form
    {
        private enum StateAlgrithm { Stopped, Reading, BuldingTree, GeneratingCode, GeneratingOutput };  //程序执行阶段不同的状态——终止，读入，建树，编码，输出
        private StateAlgrithm Current_State = StateAlgrithm.Stopped;//最初是终止的状态

        private int Current_Character; // 存储富文本框当前读取进度


        // 存储已定位节点的颜色信息
        private TreeNode First = new TreeNode();
        private TreeNode Second = new TreeNode();

        private string lastPath = "";//存取文件路径信息




        public Huffman()
        {
            InitializeComponent();//初始化
            treeView1.TreeViewNodeSorter = new OrganizeNoByFrequencyOrWeight();  //按权重排序
            ActiveControl = richTextBox_input;
        }


        #region  输入操作

        private void button1_Click(object sender, EventArgs e)//导入文本文件
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                richTextBox_input.ForeColor = SystemColors.ControlText;
                richTextBox_input.Text = File.ReadAllText(openFileDialog1.FileName, Encoding.GetEncoding("utf-8")); //编码utf-8
            }
        }






        #endregion




        #region  按钮控制操作


        private void richTextBox_input_Enter(object sender, EventArgs e)//获取焦点
        {
            if (richTextBox_input.ForeColor == SystemColors.GrayText && richTextBox_input.Text == "可以直接输入(⊙o⊙)…")
            {
                richTextBox_input.ForeColor = SystemColors.WindowText;  //由灰变黑
                richTextBox_input.Clear();//获取焦点预设提示文字清空
            }
        }



        private void richTextBox_input_Leave(object sender, EventArgs e)//失去焦点
        {
            if (string.IsNullOrEmpty(richTextBox_input.Text))
            {
                richTextBox_input.Text = "可以直接输入(⊙o⊙)…";
                richTextBox_input.ForeColor = SystemColors.GrayText;
            }
        }

        private void button2_Click(object sender, EventArgs e)  //点击开始按钮
        {
            if (button2.Text == "开始") // 开始
            {
                if (richTextBox_input.ForeColor == SystemColors.GrayText)
                {
                    MessageBox.Show(this, "请输入文本", "无法开始运行");
                    return;
                }

             
                Current_Character = 0;
                Current_State = StateAlgrithm.Stopped;
                Next_Algrithm_State();

                
                button2.Text = "终止";
                button3.Text = "暂停";
                button3.Enabled = true;
                button4.Enabled = true;
                trackBar1_Scroll(null, null);


               
                treeView1.Nodes.Clear();//树清空
                dataGridView1.Rows.Clear();//表清空
                richTextBox_output.Clear();//哈夫曼编码清空




                
                button1.Enabled = false;
                richTextBox_input.ReadOnly = true;
                ChangeSizeSourceOutput();//更改字体输出大小
                SubstituirWindows_utf8_InvalidosDaEntrada();// 替换Windows utf8 无效条目

                // Start steps
                timer1_Tick(null, null);
                timer1.Start();
            }

            else   //开始按钮显示终止
            {
                // Reset variables
                Current_State = StateAlgrithm.Stopped;
                label4.Text = "当前步骤：程序结束...";

                // Restart controls
                timer1.Stop();

                button1.Enabled = true;

                richTextBox_input.ReadOnly = false;
                richTextBox_input.SelectAll();
                richTextBox_input.SelectionBackColor = SystemColors.Window;

                button2.Text = "开始";
                button3.Text = "停止";
                button3.Enabled = false;
                button4.Enabled = false;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e) //拖动速度条
        {
            timer1.Interval = trackBar1.Maximum + 1 - trackBar1.Value;
            label5.Text = "执行速度：\n" + timer1.Interval + "ms / 每一步";
        }








        private void button3_Click(object sender, EventArgs e)  //点击暂停按钮
        {
            if (button2.Text == "开始")
            {
                return;
            }

            if (button3.Text == "暂停") // 暂停
            {
                timer1.Stop();//计时停止

                button3.Text = "继续";
            }
            else                                     // 显示的是继续
            {
                timer1.Start();//计时开始

                button3.Text = "暂停";    //点击后就显示暂停
            }
        }

        private void button4_Click(object sender, EventArgs e)  //点击下一步
        {
            if (button2.Text == "开始")
            {
                return;
            }

            button3.Text = "继续";
            timer1.Stop();           // 计时停止
            timer1_Tick(null, null); // 进行下一步
        }

        #endregion




        #region  执行算法步骤

        private void timer1_Tick(object sender, EventArgs e)//计时器
        {
            switch (Current_State)
            {
                case StateAlgrithm.Reading://读入
                    Read();
                    break;
                case StateAlgrithm.BuldingTree:  //建树
                    BuildingTree();
                    break;
                case StateAlgrithm.GeneratingCode:  //编码
                    GenerateCode();
                    break;
                case StateAlgrithm.GeneratingOutput:  //输出
                    GenerateOutput();
                    break;
                case StateAlgrithm.Stopped:    //终止
                    button2_Click(null, null);
                    break;
            }
        }

        private void Next_Algrithm_State()
        {
            switch (Current_State)
            {
                case StateAlgrithm.Stopped:
                    Current_State = StateAlgrithm.Reading;
                    label4.Text = "当前步骤：读入字符串和计算频率...";
                    break;
                case StateAlgrithm.Reading:
                    Current_State = StateAlgrithm.BuldingTree;
                    label4.Text = "当前步骤：构建哈夫曼树...";
                    break;
                case StateAlgrithm.BuldingTree:
                    Current_State = StateAlgrithm.GeneratingCode;
                    label4.Text = "当前步骤：哈夫曼编码...";
                    break;
                case StateAlgrithm.GeneratingCode:
                    Current_State = StateAlgrithm.GeneratingOutput;
                    label4.Text = "当前步骤：编码输出...";
                    Current_Character = 0;
                    break;
                case StateAlgrithm.GeneratingOutput:
                    Current_State = StateAlgrithm.Stopped;
                    label4.Text = "当前步骤：程序结束...";
                    break;
            }
        }


        private void Read()  //读入
        {
            dataGridView1.ClearSelection();//表格清空

            // StateAlgrithm.Reading
            if (Current_Character == richTextBox_input.Text.Length)//读到末尾了
            {
                //进行当前字符的选择
                richTextBox_input.Select(Current_Character - 1, 1);//选中上一个字符
                richTextBox_input.SelectionColor = SystemColors.ControlText;
                richTextBox_input.SelectionBackColor = SystemColors.Window;

                // 到达下一个程序执行状态
                Next_Algrithm_State();
                timer1_Tick(null, null); // 下一步开始创建哈夫曼树的第一步，插入松散的各个节点

                return; // 所有字符都被解析完毕           
            }

            try
            {
                LockWindowUpdate(richTextBox_input.Handle);//锁定窗口，阻止更新
                LockWindowUpdate(treeView1.Handle);//哈夫曼树窗口句柄

                //移除先前字符的高亮显示
                if (Current_Character != 0)
                {
                    richTextBox_input.Select(Current_Character - 1, 1);
                    richTextBox_input.SelectionBackColor = SystemColors.Window;
                }

                // 高亮显示当前字符           
                richTextBox_input.Select(Current_Character, 1);
                richTextBox_input.SelectionColor = SystemColors.HighlightText;
                richTextBox_input.SelectionBackColor = Color.LightSeaGreen;

                richTextBox_input.ScrollToCaret();
            }
            finally
            {
                LockWindowUpdate(IntPtr.Zero);//锁定

                // 增加字符行或者增加字符频率

                char character = richTextBox_input.Text[Current_Character];//当前字符
                int i;
                // 如果选择的字符在字符表中，那么就增加其频率
                for (i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (character == Convert.ToInt32(dataGridView1.Rows[i].Tag))
                    {
                        dataGridView1.Rows[i].Cells[1].Value = (int)dataGridView1.Rows[i].Cells[1].Value + 1;//频率加1

                        // 突出显示
                        dataGridView1.Rows[i].Selected = true;
                        richTextBox_input.SelectionColor = dataGridView1.Rows[i].DefaultCellStyle.ForeColor;//当前字符的那一行颜色突出显示
                        break;
                    }
                }

                // 如果选择的字符不在字符表中，那么就产生新的一行
                if (i == dataGridView1.RowCount)
                {
                    dataGridView1.Rows.Add(null, 1); //  添加行
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = (int)character; //添加现在的字符

                    // 字符列
                    if (character == '\n')
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = "New line ";  //跳行
                    }
                    else if (character == ' ')
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = "Space"; //空格
                    }
                    else if (char.IsControl(character)) // 显示控制字符，进行转义
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = $"({(int)character})";  //转义操作!!!!!
                    }
                    else
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = character;   //正常字符
                    }

                    dataGridView1.Rows[dataGridView1.RowCount - 1].Selected = true;


                    // 突出显示
                    Random random = new Random();//时间种子
                    Color ColorRandom = Color.FromArgb(25 + random.Next(150), 25 + random.Next(150), 25 + random.Next(150));
                    dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.ForeColor = ColorRandom;
                    richTextBox_input.SelectionColor = ColorRandom;
                }

                // 字符表根据字符出现频率排序，降序
                DataGridViewColumn frequencys = dataGridView1.Columns[1];
                dataGridView1.Sort(frequencys, ListSortDirection.Descending);

                Current_Character++;
            }
        }



        private void BuildingTree()   //构建哈夫曼树
        {
            if (treeView1.GetNodeCount(false) == 0) // 树节点是空的，没有树节点
            {
                // 首先，加入所有排好序的节点               
                foreach (DataGridViewRow line in dataGridView1.Rows)
                {
                    string CharacterName = line.Cells[0].Value.ToString();  //字符名
                    int valueCharacter = Convert.ToInt32(line.Tag);  //字符序号
                    int frequency = Convert.ToInt32(line.Cells[1].Value);  //字符频率
                    TreeNode no = new TreeNode();  //树节点
                    no.Tag = new Leaf((char)valueCharacter, frequency);   //节点的数据结构包含对应字符序号和频率
                    no.Text = $"({frequency}) {(CharacterName.Length == 1 ? $"'{CharacterName}'" : CharacterName)}";  //(出现频率)
                    no.ToolTipText = $"频率: {frequency}字符: {(CharacterName.Length == 1 ? $"'{CharacterName}'" : CharacterName)}";
                    no.ForeColor = line.DefaultCellStyle.ForeColor;
                    no.NodeFont = new Font("Consolas", 10, System.Drawing.FontStyle.Bold);
                    treeView1.Nodes.Add(no);
                }

                if (treeView1.GetNodeCount(false) > 1)//加入节点后，树视图中节点数大于1
                {
                    treeView1.Nodes[0].BackColor = SystemColors.ControlLight;
                    treeView1.Nodes[1].BackColor = SystemColors.ControlLight;
                }
            }
            else if (treeView1.GetNodeCount(false) > 1)// 树视图中节点数大于1（不包括子节点）          
            {

                // 去掉已定位节点的高光突出显示
                First.BackColor = SystemColors.Window;
                Second.BackColor = SystemColors.Window;

                // 首先检索两个树节点 (最亮的那两个)
                First = treeView1.Nodes[0];//加入
                Second = treeView1.Nodes[1];//加入


                // 上一步加入树中的节点给予高亮突出显示
                First.BackColor = SystemColors.ControlLight;
                Second.BackColor = SystemColors.ControlLight;


                // 从集合中移除掉权值最小的两个节点（已加入到树中）
                treeView1.Nodes.RemoveAt(0);
                treeView1.Nodes.RemoveAt(0);

                // Retrieves the sum of the weights of the two nodes
                int WeightFirstNo = ((No)First.Tag).Weight;
                int WeightSecondNo = ((No)Second.Tag).Weight;
                int Sum_WeightsFrequencys = WeightFirstNo + WeightSecondNo;//两节点权重相加

                //创建上面两个节点的父节点，其权重为二者之和
                TreeNode treeNodePa = new TreeNode($"({Sum_WeightsFrequencys})");
                treeNodePa.ToolTipText = $"权重: {Sum_WeightsFrequencys}";
                treeNodePa.Tag = new No(Sum_WeightsFrequencys);
                treeNodePa.Nodes.AddRange(new TreeNode[] { First, Second });

                treeView1.Nodes.Add(treeNodePa); //把新建的树加入到集合中

                treeView1.SelectedNode = treeView1.Nodes[treeView1.GetNodeCount(false) - 1];//获取分配给树视图控件的树节点的集合
                treeView1.Sort(); // 按权重降序排列


                // 对下一步会被定位的节点进行高亮
                if (treeView1.GetNodeCount(false) > 1)
                {
                    treeView1.Nodes[0].BackColor = Color.LightSeaGreen;
                    treeView1.Nodes[1].BackColor = Color.LightSeaGreen;
                }
            }
            else // 只剩下最后一个根节点，构建完成        
            {
                First.BackColor = SystemColors.Window;
                Second.BackColor = SystemColors.Window;

                Next_Algrithm_State();//转到程序下一步骤——哈夫曼编码
            }

            treeView1.ExpandAll();//展开所有节点
            treeView1.Nodes[0].EnsureVisible();//确保可见
        }


        private void GenerateCode()  //编码
        {
            if (treeView1.Nodes[0].GetNodeCount(false) == 0)//没有子节点，是叶节点

            {
                treeView1.Nodes[0].Text = "0: " + treeView1.Nodes[0].Text;
                dataGridView1.Rows[0].Cells[2].Value = "0";//右子树为0
            }
            else
            {
                Walking("0", treeView1.Nodes[0].Nodes[0]);
                Walking("1", treeView1.Nodes[0].Nodes[1]);
            }
            Next_Algrithm_State();
        }


        private void Walking(string PathWay, TreeNode No)//朝左，朝右
        {
            No.Text = PathWay[PathWay.Length - 1] + ": " + No.Text;
            No.ToolTipText = "子树: " + PathWay[PathWay.Length - 1] + " " + No.ToolTipText;

            if (No.Tag is Leaf)
            {
                foreach (DataGridViewRow line in dataGridView1.Rows)
                {
                    if ((int)line.Tag == ((Leaf)No.Tag).Character)
                    {
                        line.Cells[2].Value = PathWay;
                        break;
                    }
                }
            }
            else // 如果不是叶节点，那么就是父节点，有两个子节点           
            {
                Walking(PathWay + "0", No.Nodes[0]);
                Walking(PathWay + "1", No.Nodes[1]);
            }
        }


        private void GenerateOutput()//输出
        {
            dataGridView1.ClearSelection();//清除字符表的选择

            if (treeView1.Nodes[0].GetNodeCount(false) != 0)
                Painting(lastPath, treeView1.Nodes[0], true); // 一棵正常的树           
            else
                Painting("", treeView1.Nodes[0], true);     //对于只有一个节点的树,路径为空


            // 结束当前状态，进入下一个运行状态——终止
            if (Current_Character == richTextBox_input.Text.Length)  //编码输出完成
            {

                richTextBox_input.Select(Current_Character - 1, 1);
                richTextBox_input.SelectionBackColor = SystemColors.Window;

                // 进入下一阶段
                Next_Algrithm_State();
                timer1_Tick(null, null);//计时器继续启动
                return; //所有字符均编码输出完成
            }

            try
            {
                LockWindowUpdate(richTextBox_input.Handle);//锁定
                LockWindowUpdate(treeView1.Handle);

                //移除上个字符的高光突出显示
                if (Current_Character != 0)
                {
                    richTextBox_input.Select(Current_Character - 1, 1);
                    richTextBox_input.SelectionBackColor = SystemColors.Window;
                }

                // 对当前原本字符高光突出显示
                richTextBox_input.Select(Current_Character, 1);
                richTextBox_input.SelectionBackColor = Color.LightSeaGreen;

                richTextBox_input.ScrollToCaret();
            }
            finally
            {
                LockWindowUpdate(IntPtr.Zero);

                // 找到字符表中对应的当前字符              
                char character = richTextBox_input.Text[Current_Character];
                foreach (DataGridViewRow line in dataGridView1.Rows)
                {
                    if (character == Convert.ToInt32(line.Tag))
                    {
                        string Code = line.Cells[2].Value.ToString();

                        // 编码输出
                        richTextBox_output.AppendText(Code);

                        //给予输出编码对应字符的颜色（之前随机生成的）                      
                        int positionStartCode = richTextBox_output.Text.Length - Code.Length;
                        richTextBox_output.Select(positionStartCode, Code.Length);
                        richTextBox_output.SelectionColor = line.DefaultCellStyle.ForeColor;

                        // 对哈夫曼树中对应的节点高亮突出显示
                        if (treeView1.Nodes[0].GetNodeCount(false) != 0)//正常的树
                            Painting(Code, treeView1.Nodes[0], false);

                        else//只有一个节点的树
                            Painting("", treeView1.Nodes[0], false);

                        lastPath = Code; // 保存路径，下一步即将销毁                     

                        //字符表中对应行高亮显示                      
                        line.Selected = true;
                        break;
                    }
                }

                Current_Character++;
            }
        }
        private void Painting(string PathWay, TreeNode No, bool CleaningUp)  //richTextBox_output输出
        {
            // 最终
            if (PathWay.Length == 0 || No.GetNodeCount(false) == 0)
            {
                if (!CleaningUp)
                {
                    No.BackColor = Color.LightSeaGreen;
                }
                else
                {
                    No.BackColor = Color.Empty;
                }
                return;
            }

            //  还剩有节点
            if (!CleaningUp)
            {
                No.BackColor = SystemColors.ControlLight;
            }
            else
            {
                No.BackColor = Color.Empty;
            }
            Painting(PathWay.Substring(1), No.Nodes[int.Parse(PathWay[0].ToString())], CleaningUp);
        }



        private void richTextBox_output_TextChanged(object sender, EventArgs e)
        {
            string binarios = richTextBox_output.Text;

            if (binarios.Length < 8)
            {
                return;
            }



        }
        #endregion




        #region  一些辅助操作



        [System.Runtime.InteropServices.DllImport("user32.dll")]//  防止rtb中的大部分闪烁
        public static extern bool LockWindowUpdate(IntPtr windowLock);


        private void SubstituirWindows_utf8_InvalidosDaEntrada()//按utf8编码
        {
            byte[] bytesEntradaEmWindows_utf8 = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding("utf-8"), Encoding.Unicode.GetBytes(richTextBox_input.Text));
            richTextBox_input.Text = Encoding.GetEncoding("utf-8").GetString(bytesEntradaEmWindows_utf8);
        }

        public class OrganizeNoByFrequencyOrWeight : IComparer  //来自System.Collections，比较权重，按顺序返回
        {
            public int Compare(object noX, object noY)
            {
                // 恢复权重
                int valueX = ((No)((TreeNode)noX).Tag).Weight;
                int valueY = ((No)((TreeNode)noY).Tag).Weight;

                return valueX.CompareTo(valueY - 1); // -1:保持结构稳定（防止更改相同权重的节点顺序）
            }
        }


        #endregion




        #region   可视化

        private void ChangeSizeSourceOutput()
        {
            // 根据输入框字体大小确定输出框字体大小
            int Text_Width = 1 + TextRenderer.MeasureText(richTextBox_input.Text, new Font("Consolas", 20)).Width;
            if (Text_Width < richTextBox_input.Width * 2.3)//2.3倍
            {
                richTextBox_output.Font = new Font("Consolas", 16);
            }
            else
            {
                richTextBox_output.Font = new Font("Consolas", 10);
            }
        }

        private void dataGridView1_RowStateChanged_1(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected)
                return;

            // TODO: 将哈夫曼树转换为编码
        }



        #endregion
    }
}