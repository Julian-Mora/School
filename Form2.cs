using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife2._0
{
    public partial class Form2 : Form
    {
        int time = 0;
        int width = 0;
        int height = 0;
        public Form2()
        {
            InitializeComponent();
        }

        public int getTime()
        {
            return time;
        }
        public int getHeight()
        {
            return height;
        }

        public int getWidth()
        {
            return width;
        }

        public void setTime(int _time)
        {
            time = _time;
        }

        public void setHeight(int _height)
        {
            height = _height;
        }

        public void setWidth(int _width)
        {
            width = _width;
        }



    }
}
