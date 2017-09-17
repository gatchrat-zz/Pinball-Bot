using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace PinballBot
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        bool up = true;
        bool running = false;
        int foundValue = 0;
        int foundValueX = 0;
        public Form1()
        {
            InitializeComponent();
           
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

            //Pos-X = 319 on Spawn always 0002506C
            //Pos-Y = 40 on spawn 
            //Search memory
            String Name = "Pinball";
            int startValue = 0x254400F;
            MemoryReader mem = new MemoryReader();
            label5.Text = "Searching Memory";
            for (int i = 0; i < 16; i++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        //Zweit und dritt letzte stelle wird durchlaufen
                        startValue = 0x254400F + i * (16 * 16) + y * (16) + x ;
                        if (mem.getsingleValue(Name, startValue, "Byte") == 40 && x == 0)
                        {
                            foundValue = startValue;
                        }
                        if (mem.getsingleValue(Name, startValue, "Int") == 319)
                        {
                            foundValueX = startValue;
                        }
                    }
                   
                }
                
            }
            if (foundValue == 0 || foundValueX ==0)
            {
                label5.Text = "Memory not found";
                //Error
            }
            else
            {
                label5.Text = "Correct Memory Found :)";
               
                running = true;
            }
           


        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MemoryReader mem1 = new MemoryReader();
            String Name = "Pinball";
            int[] offsets = { 0, 0 };
             if (running)
             {
                int valueX = (int)mem1.getsingleValue(Name, foundValueX, "Int");
                int value = (int)mem1.getsingleValue(Name, foundValue, "Byte");
                label2.Text = (value).ToString();
                label4.Text = (valueX).ToString();
                Process myProcess = Process.GetProcessesByName("Pinball").FirstOrDefault();
                if (value <23 && value > 15)
                {
                    
                   
                    if (up) {
                        if (valueX>175)
                        {
                            PostMessage(myProcess.MainWindowHandle, 0x0100, 0x55, 0);
                            System.Threading.Thread.Sleep(50);
                        }
                        else
                        {
                            PostMessage(myProcess.MainWindowHandle, 0x0100, 0x5A, 0);
                            System.Threading.Thread.Sleep(50);
                        }
                        
                        up = false;
                    }
                    else
                    {
                        PostMessage(myProcess.MainWindowHandle, 0x0101, 0x55, 0);
                        PostMessage(myProcess.MainWindowHandle, 0x0101, 0x5A, 0);
                        up = true;
                    }
                }
                else
                {
                    PostMessage(myProcess.MainWindowHandle, 0x0101, 0x55, 0);
                    PostMessage(myProcess.MainWindowHandle, 0x0101, 0x5A, 0);
                    up = true;
                }
             }
        }
    }
}
