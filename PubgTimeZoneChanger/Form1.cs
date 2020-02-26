using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PubgTimeZoneChanger.Properties;

namespace PubgTimeZoneChanger
{
    public partial class Form1 : Form
    {
        private const string BackupFile = "time_zone_bk.txt";

        public Form1()
        {
            InitializeComponent();

            label1.Text = string.Format(Resources.form_now_server, ConvertServerName(TimeZoneInfo.Local));

            comboBox1.Items.Add("日本サーバー");
            comboBox1.Items.Add("アジアサーバー");
            comboBox1.Items.Add("元に戻す");

            comboBox1.SelectedIndex = 0;

            if (!File.Exists(BackupFile))
            {
                File.WriteAllText(BackupFile, TimeZoneInfo.Local.Id);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("コンピュータの時間が変更されてしまいますが、変更しますか？", "注意", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                string locName = ConvertFromServerName(comboBox1.Items[comboBox1.SelectedIndex].ToString());
                ProcessStartInfo startInfo =
                    new ProcessStartInfo("tzutil.exe",
                        $"/s \"{locName}\"");
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                Process process = Process.Start(startInfo);

                process?.WaitForExit();

                if (process?.ExitCode == 0)
                {
                    MessageBox.Show("成功しました!", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    label1.Text = string.Format(Resources.form_now_server,
                        ConvertServerName(TimeZoneInfo.FindSystemTimeZoneById(locName)));
                }
                else
                {
                    MessageBox.Show(process?.StandardOutput.ReadToEnd(), "エラー", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private string ConvertServerName(TimeZoneInfo timeZoneInfo)
        {
            string stdName = timeZoneInfo.Id.Replace("_dstoff", "");
            switch (stdName)
            {
                case "Tokyo Standard Time":
                    return "日本サーバー";
                case "China Standard Time":
                    return "アジアサーバー";
                default:
                    return "その他";
            }
        }

        private string ConvertFromServerName(string serverName)
        {
            switch (serverName)
            {
                case "日本サーバー":
                    return "Tokyo Standard Time";
                case "アジアサーバー":
                    return "China Standard Time";
                case "元に戻す":
                    return File.ReadAllText(BackupFile);
                default:
                    throw new Exception("変換に失敗");
            }
        }
    }
}