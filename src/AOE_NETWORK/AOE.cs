/** 李辉 1652286 **/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AOE_NETWORK
{

    #region  AOE类，主要部分
    public partial class AOE : Form
    {
        public Network AOE_NET = new Network();                 // AOE网
        public List<Button> events = new List<Button>();       // 事件按钮
        public List<int> act = new List<int>();                // 活动上的一对事件
        bool creat;                                            // 是否可创建工程
        public Graphics g;
        

        public AOE()
        {
            InitializeComponent();
            creat = false;
            g = this.panel1.CreateGraphics();
            button2.Enabled = false;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)  //开始创建
        {
            for (int count = 0; count != 4 * (tableLayoutPanel2.RowCount - 1); ++count)
            {
                Control temp = new Control();
                foreach (Control c in tableLayoutPanel2.Controls)
                {
                    if (c is Label)
                        if (c.Name != "label1" && c.Name != "label3" &&
                            c.Name != "label4" && c.Name != "label2")
                        {
                            temp = c;
                            break;
                        }

                }
                tableLayoutPanel2.Controls.Remove(temp);
            }

            panel1.Controls.Clear();
            g.Clear(panel1.BackColor);
            AOE_NET = new Network();
            events = new List<Button>();
            act = new List<int>();
            tableLayoutPanel2.RowCount = 1;
            labelEarliest.Text = "工程最早完成时间";
            creat = true;
            button2.Enabled = true;
        }


       


        private void panel1_MouseClick(object sender, MouseEventArgs e)  //画AOE网络
        {
            if (creat)
            {
                if (e.X > (panel1.Location.X + 5) &&
                    e.X < (panel1.Location.X + panel1.Size.Width - 10) &&
                    e.Y > (panel1.Location.Y - 10) &&
                    e.Y < (panel1.Location.Y + panel1.Size.Height - 25))
                {
                    Event eve = new Event();
                    AOE_NET.net.Add(eve);

                    Point p = new Point(e.X - 15, e.Y - 15);
                    CircleButton new_event = new CircleButton();
                    new_event.Name = "buttonEvent" + events.Count.ToString();
                    new_event.Location = p;
                    new_event.Width = 29;
                    new_event.Height = 29;
                    new_event.Text = (events.Count + 1).ToString();
                    new_event.TextAlign = ContentAlignment.MiddleCenter;
                    new_event.FlatStyle = FlatStyle.Flat;
                    new_event.MouseClick += new MouseEventHandler(this.eventClick);

                    events.Add(new_event);
                    panel1.Controls.Add(new_event);
                }
            }
        }




        private void eventClick(object sender, MouseEventArgs e)  //点击事件
        {
            Button b_clicked = (Button)sender;

            if (act.Count == 0)
            {
                act.Add(events.FindIndex((Button b) => b.Name == b_clicked.Name));
                //act.Add(int.Parse((b_clicked.Text[b_clicked.Text.Length - 1]).ToString()) - 1);
            }
            else if (act.Count == 1)
            {
                if ((events.FindIndex((Button b) => b.Name == b_clicked.Name)) != act[0])
                {
                    act.Add(events.FindIndex((Button b) => b.Name == b_clicked.Name));
                    bool flag = false;
                    for (int index = 0; index != AOE_NET.net.Count; ++index)
                        if (((AOE_NET.net)[index].next.Find((Activity a) => (a.start == act[0] && a.end == act[1]))) != null)
                            flag = true;

                    if (flag == false)
                    {
                        Act_Time cost = new Act_Time(this);
                        cost.ShowDialog();
                        act.Clear();
                        panel1.Controls.Add(cost.label);
                    }
                    else
                    {
                        MessageBox.Show("事件已存在。");
                        act.Clear();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)//执行计算
        {
            for (int index = 0; index != events.Count; ++index)
                events[index].Enabled = false;
            creat = false;
            String m = AOE_NET.Is_Able_Continue();
            if (m != null)
            {
                MessageBox.Show(m + "请重新创建工程。");
                button2.Enabled = false;
                return;
            }
            AOE_NET.eventsTime();
            AOE_NET.activitiesTime(this);

            labelEarliest.Text = labelEarliest.Text + "： " + AOE_NET.net[AOE_NET.end].e;

            for (int index = 0; index != AOE_NET.net.Count; ++index)
            {
                for (int i = 0; i != (AOE_NET.net)[index].next.Count; ++i)
                {
                    ++tableLayoutPanel2.RowCount;

                    Label la = new Label();
                    la.TextAlign = ContentAlignment.MiddleCenter;
                    la.Font = new Font("Microsoft YaHei", 9);
                    la.Dock = DockStyle.Fill;
                    la.Text = ((AOE_NET.net)[index].next[i].start + 1).ToString() +
                              " => " + ((AOE_NET.net)[index].next[i].end + 1).ToString();

                    Label lc = new Label();
                    lc.TextAlign = ContentAlignment.MiddleCenter;
                    lc.Font = new Font("Microsoft YaHei", 9);
                    lc.Dock = DockStyle.Fill;
                    lc.Text = (AOE_NET.net)[index].next[i].weight.ToString();

                    Label le = new Label();
                    le.TextAlign = ContentAlignment.MiddleCenter;
                    le.Font = new Font("Microsoft YaHei", 9);
                    le.Dock = DockStyle.Fill;
                    le.Text = (AOE_NET.net)[index].next[i].e.ToString();

                    Label ll = new Label();
                    ll.TextAlign = ContentAlignment.MiddleCenter;
                    ll.Font = new Font("Microsoft YaHei", 9);
                    ll.Dock = DockStyle.Fill;
                    ll.Text = (AOE_NET.net)[index].next[i].l.ToString();

                   

                    if (((AOE_NET.net)[index].next)[i].critical == true)
                    {
                       
                        la.BackColor = Color.Red;
                        lc.BackColor = Color.Red;
                        le.BackColor = Color.Red;
                        ll.BackColor = Color.Red;
                        
                    }

                    tableLayoutPanel2.Controls.Add(la, 0, tableLayoutPanel2.RowCount - 1);
                    tableLayoutPanel2.Controls.Add(lc, 1, tableLayoutPanel2.RowCount - 1);
                    tableLayoutPanel2.Controls.Add(le, 2, tableLayoutPanel2.RowCount - 1);
                    tableLayoutPanel2.Controls.Add(ll, 3, tableLayoutPanel2.RowCount - 1);

                }
            }

            button2.Enabled = false;
        }

     
    }

    #endregion


    #region 圆形按钮
    public class CircleButton : Button//继承按钮类 
    {
        protected override void OnPaint(PaintEventArgs e)//重新设置控件的形状   protected 保护  override重新
        {
            base.OnPaint(e);//递归  每次重新都发生此方法,保证其形状为自定义形状
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(2, 2, this.Width - 6, this.Height - 6);
            Graphics g = e.Graphics;
            g.DrawEllipse(new Pen(Color.Black, 2), 2, 2, Width - 6, Height - 6);
            Region = new Region(path);
        }
    }
    #endregion


    #region  Activity类，活动
    public class Activity  
    {
        public int start;        // 活动起始事件序号
        public int end;        // 活动结束事件序号
        public int weight;     // 活动持续时间，权重
        public int e;          // 活动最早开始时间
        public int l;          // 活动最迟开始时间
        public bool critical;   // 是否为关键活动
        public Point p_start = new Point();   // 活动起点坐标(画图用)
        public Point p_end = new Point();   // 活动终点坐标(画图用)

        public Activity(int start, int end, int weight, Point p_start, Point p_end)//构造
        {
            this.start = start;
            this.end = end;
            this.weight = weight;
            this.critical = false;//初始无关键路径
            this.p_start = p_start;
            this.p_end = p_end;
        }
    }
    #endregion


    #region  Event类，事件
    public class Event 
    {
        public bool critical;                                // 是否为关键路径上的事件
        public List<Activity> next = new List<Activity>();  // 以该事件为开始的活动
        public int ins;                                      // 入度
        public int e;      // 事件最早发生时间
        public int l;      // 事件最迟发生时间

        public Event()//构造
        {
            critical = false;
            ins = 0;
            e = 0;
            l = 0;
        }
    }
    #endregion


    #region  Network类，网络
    public class Network
    {
        Graphics g;
        public List<Event> net = new List<Event>();         // AOE网邻接表
        public int start;    // 工程起始事件序号
        public int end;    // 工程结束事件序号
        List<int> topology = new List<int>();               // 拓扑排序结果

        public Network()
        {
            start = -1;
            end = -1;
        }

       
        public String Is_Able_Continue()   // 求解工程是否可行，可继续
        {
            int count;

            // 工程中不含事件
            count = 0;
            for (int index = 0; index != net.Count; ++index)
                if (net[index].next.Count != 0)
                    ++count;
            if (count == 0)
                return "工程中不存在活动。"; 

            // 工程有多个源点
            count = 0; 
            for (int index = 0; index != net.Count; ++index)
                if (net[index].ins == 0)
                    ++count;
            if (count > 1)
                return "工程有多个源点。";

            // 工程有多个汇点
            count = 0;
            for (int index = 0; index != net.Count; ++index)
                if (net[index].next.Count == 0)
                    ++count;
            if (count > 1)
                return "工程有多个汇点。";

            // 工程中含有向环
            List<int> temp = new List<int>();
            topology = new List<int>();
            for (int index = 0; index != net.Count; ++index)
                temp.Add(net[index].ins);
            count = 0;
            for (int index = 0; index != temp.Count; ++index)
            {
                if (temp[index] == 0)
                {
                    topology.Add(index);
                    ++count;
                    for (int i = 0; i != net[index].next.Count; ++i)
                        --temp[net[index].next[i].end];
                    temp[index] = -1;
                    index = -1;
                }
            }
            if (count != temp.Count)
                return "AOE网络中含有环路！";
            else
            {
                start = topology[0];
                end = topology[topology.Count - 1];
                return null;
            }
        }




        public void eventsTime()   // 求各事件的最早和最迟开始时间
        {
            // 求各事件的最早开始时间
            for (int index = 0; index != topology.Count - 1; ++index)
            {
                for (int i = 0; i != net[topology[index]].next.Count; ++i)
                {
                    int time = net[topology[index]].e + (net[topology[index]].next)[i].weight;
                    if (time > net[(net[topology[index]].next)[i].end].e)
                        net[(net[topology[index]].next)[i].end].e = time;
                }
            }

            // 求各事件的最迟开始时间
            net[topology[topology.Count - 1]].l = net[topology[topology.Count - 1]].e;
            int longest = net[topology[topology.Count - 1]].l;
            for (int index = 0; index != net.Count; ++index)
                if (index == topology[0])
                    ;//不操作 
                else
                    net[index].l = longest;

            for (int index = topology.Count - 1; index != 0; --index)
            {
                // 查找当前事件的前驱
                for (int i = 0; i != net.Count; ++i)
                {
                    Activity temp = net[i].next.Find((Activity a) => a.end == topology[index]);
                    if (temp != null)
                    {
                        if (i != topology[0])
                        {
                            int time = net[topology[index]].l - temp.weight;
                            if (time < net[i].l)
                                net[i].l = time;
                        }
                    }
                }
            }
        }

      
        public void activitiesTime(AOE main)   // 求各活动的最早和最迟开始时间
        {
            for (int index = 0; index != net.Count; ++index)
                for (int i = 0; i != net[index].next.Count; ++i)
                {
                    // 最早开始时间
                    (net[index].next)[i].e = net[index].e;
                    // 最迟开始时间
                    (net[index].next)[i].l = net[(net[index].next)[i].end].l - (net[index].next)[i].weight;
                    // 关键活动
                    if ((net[index].next)[i].e == (net[index].next)[i].l)
                    {
                        (net[index].next)[i].critical = true;
                        net[(net[index].next)[i].start].critical = true;
                        net[(net[index].next)[i].end].critical = true;

                        Pen pen = new Pen(Color.Red, 4);
                        AdjustableArrowCap lineCap = new AdjustableArrowCap(4, 15);
                        pen.CustomEndCap = lineCap;
                        g = main.g;

                        g.DrawLine(pen, (net[index].next)[i].p_start,
                                        (net[index].next)[i].p_end);
                    }
                }
        }
    }
    #endregion
}
