using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb; //Allows us to connect to Access Database
using System.IO; //Allows access to streamreader and writer commands

namespace Antram_FinalProject
{
    public partial class Form1 : Form
    {
        //property to hold filepath
        private string filepath;
        //Arrays to store multiple search values
        string[] director;
        string[] cast;
        string[] keyword;

        public Form1()
        {
            InitializeComponent();
            filepath = ""; //Set filepath to blank string
            button1.Enabled = false; //Disable button until a file is selected
            richTextBox1.Text = "Please select a file\n"; //Prompt user to select a file

            //Check if summary file has been created
            try
            {
                using (var sr = new StreamReader("Antram_Summary.csv"))
                {
                    richTextBox1.AppendText("Summary file exists");
                    sr.Close();
                }
            }
            catch
            {
                using(var sw = new StreamWriter("Antram_Summary.csv"))
                {
                    sw.WriteLine("filename,castSearch,directorSearch,keywordSearch");
                    sw.Close();
                }
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {

            //Opening open file dialog to select file
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK) //Saves selected filepath
            {
                filepath = openFileDialog1.FileName;
                richTextBox1.Clear();
                button1.Enabled = true; //Re-enable button once file is selected
            }
            else //If nothing selected then filepath set to blank string
            {
                filepath = "";
                richTextBox1.Text = "Select a database.";
                button1.Enabled = false;               
            }
        }

        private void Search(object sender, EventArgs e)
        {
            //Variable to hold title value for search csv file
            string searchTitle = String.Format("{0}{1}{2}", textBox1.Text.Trim(' ') ,textBox2.Text.Trim(' ') , textBox3.Text.Trim(' '));

            //Create Search log file
            using (var sw2 = new StreamWriter(String.Format("Antram_{0}", searchTitle.Trim(' '))))
            {
                sw2.WriteLine("RealeaseYear,Title,Director,Cast");
                sw2.Close();
            }
            
            //Flag variables to check if text boxes are blank initialized to false by default
            bool directorCheck = false;
            bool castCheck = false;
            bool keywordCheck = false;

            //Logic to check if searchbox is blank
            if (textBox1.Text != "")
            {
                director = textBox1.Text.Split(' ');
                directorCheck = true;
            }

            if (textBox2.Text != "")
            {

                cast = textBox2.Text.Split(' ');
                castCheck = true;
            }

            if (textBox3.Text != "")
            {
                keyword = textBox3.Text.Split(' ');
                keywordCheck = true;
            }
            
            //Append Search Summary File
            using (var sw = new StreamWriter("Antram_Summary.csv", true))
            {
                sw.WriteLine("Antram_{0},{1},{2},{3}", searchTitle.Trim(' '), textBox2.Text, textBox1.Text, textBox3.Text);
                sw.Close();
            }

            //Connection object to make connection to Database
            string connectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", filepath);

            //We need a properly formatted connection string to connect to database
            OleDbConnection dbconn = new OleDbConnection(connectionString);
            dbconn.Open(); //Open the connection

            OleDbCommand dbCommand = new OleDbCommand(); //Allows us to execute commands to the reader

            dbCommand.CommandText = "SELECT * FROM MoviePlots";
            dbCommand.Connection = dbconn;

            OleDbDataReader dbReader = dbCommand.ExecuteReader(); //sends query to database connection to retreive data

            richTextBox1.Clear();

            while (dbReader.Read()) //While still in database
            {
                //Logic to determine if a search is complete with only one flagged search
                
                if (directorCheck && !castCheck && !keywordCheck)
                {
                    //loop to search through directors array and look for matches in file
                    for (int i = 0; i < director.Length; i++)
                    {
                        if (dbReader[4].ToString().Contains(director[i]))
                        {
                            richTextBox1.AppendText(String.Format("{0} | {1} | {2} | {3}\n", dbReader[1], dbReader[2], dbReader[4], dbReader[5]));
                        
                            //Adding double quotes around multiple search parameters
                            if (director.Length > 2)
                            {
                                //Append Search File
                                using (var sw2 = new StreamWriter(String.Format("Antram_{0}", searchTitle.Trim(' ')), true))
                                {
                                    sw2.WriteLine("{0},{1},\"{2}\",{3}", dbReader[1], dbReader[2], dbReader[4], dbReader[5]);
                                    sw2.Close();
                                }
                            }
                            else
                            {
                                //Append Search File
                                using (var sw2 = new StreamWriter(String.Format("Antram_{0}", searchTitle.Trim(' ')), true))
                                {
                                    sw2.WriteLine("{0},{1},{2},{3}", dbReader[1], dbReader[2], dbReader[4], dbReader[5]);
                                    sw2.Close();
                                }
                            }

                            break;
                        }
                    }
                }
                else if (castCheck && !directorCheck && !keywordCheck)
                {
                    //loop to search through cast array and look for matches in file
                    for (int i = 0; i < cast.Length; i++)
                    {
                        if (dbReader[5].ToString().Contains(cast[i]))
                        {
                            richTextBox1.AppendText(String.Format("{0} | {1} | {2} | {3}\n", dbReader[1], dbReader[2], dbReader[4], dbReader[5]));

                            //Adding double quotes around multiple search parameters
                            if (cast.Length > 2)
                            {
                                //Append Search File
                                using (var sw2 = new StreamWriter(String.Format("Antram_{0}", searchTitle.Trim(' ')), true))
                                {
                                    sw2.WriteLine("{0},{1},\"{2}\",{3}", dbReader[1], dbReader[2], dbReader[4], dbReader[5]);
                                    sw2.Close();
                                }
                            }
                            else
                            {
                                //Append Search File
                                using (var sw2 = new StreamWriter(String.Format("Antram_{0}", searchTitle.Trim(' ')), true))
                                {
                                    sw2.WriteLine("{0},{1},{2},{3}", dbReader[1], dbReader[2], dbReader[4], dbReader[5]);
                                    sw2.Close();
                                }
                            }

                            break;
                        }
                        
                    }
                }
                else if (keywordCheck && !directorCheck && !castCheck)
                {
                    //loop to search through keyword array and look for matches in file
                    for (int i = 0; i < keyword.Length; i++)
                    {
                        if (dbReader[8].ToString().Contains(keyword[i]))
                        {
                            richTextBox1.AppendText(String.Format("{0} | {1} | {2} | {3}\n", dbReader[1], dbReader[2], dbReader[4], dbReader[5]));

                            //Adding double quotes around multiple search parameters
                            if (keyword.Length > 2)
                            {
                                //Append Search File
                                using (var sw2 = new StreamWriter(String.Format("Antram_{0}", searchTitle.Trim(' ')), true))
                                {
                                    sw2.WriteLine("{0},{1},\"{2}\",{3}", dbReader[1], dbReader[2], dbReader[4], dbReader[5]);
                                    sw2.Close();
                                }
                            }
                            else
                            {
                                //Append Search File
                                using (var sw2 = new StreamWriter(String.Format("Antram_{0}", searchTitle.Trim(' ')), true))
                                {
                                    sw2.WriteLine("{0},{1},{2},{3}", dbReader[1], dbReader[2], dbReader[4], dbReader[5]);
                                    sw2.Close();
                                }
                            }

                            break;
                        }
                        
                    }
                }
                //Logic with 2 valid flags
                else if (directorCheck && castCheck && !keywordCheck)
                {
                    int matchcount = 0; //counts partial matches. When a whole match is reached (2 partials in this case) then print results
                    for (int i = 0; i < director.Length; i++)
                    {
                        if (dbReader[4].ToString().Contains(director[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }
                    for (int i = 0; i < cast.Length; i++)
                    {
                        if (dbReader[5].ToString().Contains(cast[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }

                    if (matchcount == 2)
                    {
                        richTextBox1.AppendText(String.Format("{0} | {1} | {2} | {3}\n", dbReader[1], dbReader[2], dbReader[4], dbReader[5]));
                    }
                    
                }
                else if (castCheck && keywordCheck && !directorCheck)
                {
                    int matchcount = 0; //counts partial matches. When a whole match is reached (2 partials in this case) then print results
                    for (int i = 0; i < keyword.Length; i++)
                    {
                        if (dbReader[8].ToString().Contains(keyword[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }
                    for (int i = 0; i < cast.Length; i++)
                    {
                        if (dbReader[5].ToString().Contains(cast[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }

                    if (matchcount == 2)
                    {
                        richTextBox1.AppendText(String.Format("{0} | {1} | {2} | {3}\n", dbReader[1], dbReader[2], dbReader[4], dbReader[5]));
                    }
                   
                }
                else if (directorCheck && keywordCheck && !castCheck)
                {
                    int matchcount = 0; //counts partial matches. When a whole match is reached (2 partials in this case) then print results
                    for (int i = 0; i < keyword.Length; i++)
                    {
                        if (dbReader[8].ToString().Contains(keyword[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }
                    for (int i = 0; i < director.Length; i++)
                    {
                        if (dbReader[4].ToString().Contains(director[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }

                    if (matchcount == 2)
                    {
                        richTextBox1.AppendText(String.Format("{0} | {1} | {2} | {3}\n", dbReader[1], dbReader[2], dbReader[4], dbReader[5]));
                    }
                   
                }
                //logic with all 3 flags valid
                else if (directorCheck && castCheck && keywordCheck)
                {
                    int matchcount = 0; //counts partial matches. When a whole match is reached (2 partials in this case) then print results
                    for (int i = 0; i < director.Length; i++)
                    {
                        if (dbReader[4].ToString().Contains(director[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }
                    for (int i = 0; i < cast.Length; i++)
                    {
                        if (dbReader[5].ToString().Contains(cast[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }
                    for (int i = 0; i < keyword.Length; i++)
                    {
                        if (dbReader[8].ToString().Contains(keyword[i]))
                        {
                            matchcount++;
                            break;
                        }
                    }

                    if (matchcount == 3)
                    {
                        richTextBox1.AppendText(String.Format("{0} | {1} | {2} | {3}\n", dbReader[1], dbReader[2], dbReader[4], dbReader[5]));
                    }
                }




            }

            dbReader.Close(); //Close dbReader so we can reuse
            dbconn.Close();//Close Connection

        }
    }
}
