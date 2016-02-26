using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CharsUtil
{
    public partial class CharsUtil : Form
    {
        public static string strlast;

        public CharsUtil()
        {
            InitializeComponent();
            this.panel2.Parent = this.outerPanel;
            this.panel3.Parent = this.outerPanel;

            this.textBoxFolder.Parent = this.panel3;
            this.buttonFolder.Parent = this.panel3;
            tabControl1.Visible = false;

            // 初期表示時に、先頭の項目を選択
            comboBoxEncode.SelectedIndex = 0;

            toolStripProgressBar1.Value = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialogクラスのインスタンスを作成
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fbd.Description = "フォルダを指定してください。";
            //ルートフォルダを指定する
            //デフォルトでDesktop
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fbd.SelectedPath = @"C:\";
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fbd.ShowNewFolderButton = true;
            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                textBoxFolder.Text = fbd.SelectedPath;

                //読み込みボタンを有効に
                buttonRead.Enabled = true;
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void 複数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = true;
            textBoxFile.Text = "";
            if (textBoxFolder.Text == "")
            {
                buttonRead.Enabled = false;
                buttonSave.Enabled = false;
            }
            else
            {
                buttonRead.Enabled = true;
                buttonSave.Enabled = true;
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void 単一ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel3.Visible = false;
            textBoxFolder.Text = "";
            if (textBoxFile.Text == "")
            {
                buttonRead.Enabled = false;
                buttonSave.Enabled = false;
            }
            else
            {
                buttonRead.Enabled = true;
                buttonSave.Enabled = true;
            }
        }

        private void buttonFile_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき
                //選択されたファイル名を表示する
                textBoxFile.Text = ofd.FileName;

                //読み込みボタンを有効に
                buttonRead.Enabled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void buttonRead_Click(object sender, EventArgs e)
        {
            buttonRead.Enabled = false;
            buttonSave.Enabled = false;
            charlist.Text = "";
            textBoxFiles.Text = "";
            label1.Text = "";
            toolStripProgressBar1.Value = 0;

            if (toolStripButton2.Checked != true)
            {
                string path = textBoxFile.Text;
                //内容を読み込む
                using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding(comboBoxEncode.Text)))
                {
                    string str = chrsRead(sr);
                    //エンコード表示
                    charlist.Text = "Encode: " + comboBoxEncode.Text + "\r\n";
                    charlist.AppendText("Count: " + str.Count() + "\r\n");
                    charlist.AppendText(str);
                    buttonSave.Enabled = true;
                    label1.Text = "完了";
                    toolStripProgressBar1.Value = 100;
                }
                buttonRead.Enabled = true;
            }
            else if (toolStripButton2.Checked == true)
            {
                buttonFolder.Enabled = false;
                await ReadAsync();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();
            //はじめのファイル名を指定する
            sfd.FileName = "chars.txt";
            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき
                // ファイル名取得
                string fpath = sfd.FileName;
                // ファイル保存
                using (StreamWriter txt = new StreamWriter(fpath, false, Encoding.GetEncoding("utf-16")))
                {
                    if (toolStripButton2.Checked != true)
                    {
                        string path = textBoxFile.Text;
                        using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding(comboBoxEncode.Text)))
                        {
                            string str = chrsRead(sr);
                            txt.Write(str);
                        }
                    }
                    else if (toolStripButton2.Checked == true)
                    {
                        string str = strlast;
                        txt.Write(str);
                    }
                }
            }
        }

        private string chrsRead(StreamReader sr)
        {
            List<char> chrs = new List<char>();
            char chr;
            try
            {
                while (!sr.EndOfStream)
                {
                    string str2 = sr.ReadLine();
                    for (int i = 0; i < str2.Length; i++)
                    {
                        chr = str2[i];
                        if (!chrs.Contains(chr))
                        {
                            chrs.Add(chr);
                        }
                    }
                }
            }
            finally
            {
                if (sr != null)
                {
                    ((IDisposable)sr).Dispose();
                }
            }
            chrs.Sort();
            //閉じる
            sr.Close();
            char[] c = chrs.ToArray();
            string str = new String(c);
            return str;
        }

        // 進捗を表示するメソッド（これはUIスレッドで呼び出される）
        private void ShowProgress(int percent)
        {
            toolStripProgressBar1.Value = percent;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel3.Visible = false;
            textBoxFolder.Text = "";
            textBoxFile.Text = "";
            if (textBoxFile.Text == "")
            {
                buttonRead.Enabled = false;
                if (charlist.Text == "")
                {
                    buttonSave.Enabled = false;
                }
            }
            else
            {
                buttonRead.Enabled = true;
                buttonSave.Enabled = true;
            }
            toolStripButton1.Checked = true;
            toolStripButton2.Checked = false;
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = true;
            textBoxFile.Text = "";
            if (textBoxFolder.Text == "")
            {
                buttonRead.Enabled = false;
                if (charlist.Text == "")
                {
                    buttonSave.Enabled = false;
                }
            }
            else
            {
                buttonRead.Enabled = true;
                buttonSave.Enabled = true;
            }
            toolStripButton2.Checked = true;
            toolStripButton1.Checked = false;
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        public async Task ReadAsync()
        {
            // Progressクラスのインスタンスを生成
            var p = new Progress<int>(ShowProgress);
            string encode = comboBoxEncode.Text;
            string path = textBoxFolder.Text;
            // カンマ区切りで拡張子を分割して配列に格納する
            string[] extarray = textBoxExt.Text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            if (extarray.Length != 0)
            {
                int filecount = 0;
                List<string> strlist = new List<string>();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                // マッチするファイルを検索
                await Task.Run(() =>
                {
                    foreach (string ext in extarray)
                    {
                        IEnumerable<string> files = Directory.EnumerateFiles(
                        path, // 検索開始ディレクトリ
                        "*." + ext, // 検索パターン
                        SearchOption.AllDirectories);
                        foreach (string file in files)
                        {
                            strlist.Add(ReadAll(file, encode));
                            Invoke(new Action(() =>
                            {
                                //textBoxFIlesにマッチしたファイルを表示
                                textBoxFiles.AppendText(file + "\r\n");
                            }));
                        }

                        filecount += files.Count();
                        stopwatch.Stop();
                    }
                });
                textBoxFiles.AppendText("ファイル数: " + filecount + "\r\n" + stopwatch.Elapsed.TotalMilliseconds + "ms" + "\r\n");

                List<char> charsum = new List<char>();
                await Task.Run(() =>
                {
                    charsum = multicharsReadind(p, strlist);
                    strlast = chardistnctLinq(charsum);
                });

                label1.Text = "完了";
                //エンコード表示
                string encodetextbox = "Encode: " + comboBoxEncode.Text + "\r\n";
                string counttextbox = "Count: " + strlast.Count() + "\r\n";

                StringBuilder uniqsb = new StringBuilder();
                uniqsb.Append(encodetextbox + counttextbox + strlast);
                charlist.Text = uniqsb.ToString();

                toolStripProgressBar1.Value = 100;

                buttonSave.Enabled = true;
                buttonFolder.Enabled = true;

                System.GC.Collect();
                // ファイナライゼーションが終わるまでスレッド待機
                System.GC.WaitForPendingFinalizers();
                // ファイナライズされたばかりのオブジェクトに関連するメモリを開放
                System.GC.Collect();
            }
            else
            {
                textBoxFiles.Text = "拡張子を指定してください\r\n";
            }
            buttonRead.Enabled = true;
            buttonSave.Enabled = true;
        }

        //Listから一文字ずつ読み込む
        private List<char> multicharsReadind(IProgress<int> progress, List<string> strlist)
        {
            List<char> charsum = new List<char>();
            List<char> chrs = new List<char>();
            char chr;
            int mcount = strlist.Count;
            int count = 1;

            foreach (string sb in strlist)
            {
                try
                {
                    chrs.Clear();

                    foreach (char charind in sb)
                    {
                        if (charind.ToString() != "\r\n " && charind.ToString() != "\n\r" && charind.ToString() != "\r" && charind.ToString() != "\n" && charind.ToString() != "  ")
                        {
                            chr = charind;
                            chrs.Add(chr);
                        }
                    }

                    charsum.AddRange(chrs);

                }
                finally
                {
                    if (sb != null)
                    {

                    }
                }
                double num = 1.1;
                int percentage = 100 * count / (int)(mcount * num); // 進捗率
                progress.Report(percentage);
                count++;
            }

            return charsum;
        }

        //ソートして重複文字を削除 6480
        private string chardistnct(List<char> chrs)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            chrs.Sort();
            char[] dis = chrs.Distinct().ToArray();
            List<char> replaced = new List<char>();
            foreach (char replace in dis)
            {
                if (!Regex.IsMatch(replace.ToString(), "[\r\n]|\t|[\u0000-\u0019]"))
                {
                    replaced.Add(replace);
                }
            }
            string str = new String(replaced.ToArray());
            stopwatch.Stop();
            MessageBox.Show(stopwatch.Elapsed.TotalMilliseconds + "ms");
            return str;
        }
        /// <summary>
        /// Linq版 6609
        /// </summary>
        /// <param name="chrs"></param>
        /// <returns></returns>
        private string chardistnctLinq(List<char> chrs)
        {
            List<char> dis = chrs.AsParallel()
                .Distinct()
                .ToList();
            dis.Sort();

            Regex regex = new Regex("[\r\n]|\t|[\u0000-\u0019]");
            char[] rep = dis.AsParallel().Where(v => !regex.IsMatch(v.ToString()))
                .ToArray();

            string str = new String(rep);
            return str;
        }

        private string streamRead(string fpath, string encode)
        {
            using (StreamReader sr = new StreamReader(fpath, Encoding.GetEncoding(encode)))
            {
                string str = sr.ReadToEnd();
                return str;
            }
        }

        private string ReadAll(string fpath, string encode)
        {
            string file = File.ReadAllText(fpath, Encoding.GetEncoding(encode));
            return file;
        }
    }
}