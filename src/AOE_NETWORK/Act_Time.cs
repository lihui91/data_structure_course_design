using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AOE_NETWORK
{
    public partial class Act_Time : Form
    {
        public Act_Time(AOE main)
        {
            InitializeComponent();
            this.main = main;
            g = main.g;
        }

        public Label label = new Label();
        public AOE main;
        Graphics g;


        private void Act_Time_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)//返回
        {
            main.act.Clear();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int weight = Convert.ToInt32(textBox1.Text);
                if (weight <= 0)
                {
                    MessageBox.Show("错误！请输入大于0的整数！");
                   return;
                }

                Point start = new Point();
                start.X = main.events[main.act[0]].Location.X + 12;
                start.Y = main.events[main.act[0]].Location.Y + 12;

                Point end = new Point();
                end.X = main.events[main.act[1]].Location.X + 12;
                end.Y = main.events[main.act[1]].Location.Y + 12;

                Activity act = new Activity(main.act[0], main.act[1], weight, start, end);
                main.AOE_NET.net[main.act[0]].next.Add(act);
                ++main.AOE_NET.net[main.act[1]].ins;

                Pen pen = new Pen(Color.Blue, 2);
                AdjustableArrowCap lineCap = new AdjustableArrowCap(3, 10);
                pen.CustomEndCap = lineCap;
                g.DrawLine(pen, start, end);


                label.Text = textBox1.Text;
                label.Location = new Point((start.X + end.X) / 2-15, (start.Y + end.Y) / 2-7);
                label.Size = new Size(30, 15);

                main.act.Clear();
                this.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("错误信息：" + ee.Message +
                                "\n请输入大于0的整数！");
            }
        }
    }
}
