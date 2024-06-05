using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

namespace ExchangeRates
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Visible = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            string day1 = dateTime.Day.ToString();
            string month1 = dateTime.Month.ToString();
            string year1 = dateTime.Year.ToString();
            string date1 = day1 + month1 + year1;

            string day2 = dateTimePicker1.Value.Day.ToString();
            string month2 = dateTimePicker1.Value.Month.ToString();
            string year2 = dateTimePicker1.Value.Year.ToString();
            string date2 = day2 + month2 + year2;

            string url = "https://www.tcmb.gov.tr/kurlar/";
            if (date1 != date2)
            {
                if (day2.Length < 2)
                {
                    day2 = "0" + day2;
                }
                if (month2.Length < 2)
                {
                    month2 = "0" + month2;
                }
                url += year2 + month2 + "/" + day2 + month2 + year2 + ".xml";
            }

            else
            {
                url += "today.xml";
            }

            dataGridView1.DataSource = null;
            try
            {
                dataGridView1.DataSource = kurOku(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kurlar Okunamadı \n" + ex.Message.ToString(), "HATA !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            dataGridView1.RowPrePaint += dataGridView1_RowPrePaint;

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.AutoResizeColumns();
                dataGridView1.ClearSelection();
                label1.Text = dateTimePicker1.Text.ToString() + " Tarihli TCMB Kur Bilgileri";
                label1.Visible = true;
            }
        }

        private DataTable kurOku(string url)
        {
            DataTable dt = new DataTable();
            DataRow dr;

            dt.Columns.Add(new DataColumn("Döviz Adı", typeof(string)));
            dt.Columns.Add(new DataColumn("Döviz Kodu", typeof(string)));
            dt.Columns.Add(new DataColumn("Birim", typeof(string)));
            dt.Columns.Add(new DataColumn("Döviz Alış", typeof(string)));
            dt.Columns.Add(new DataColumn("Döviz Satış", typeof(string)));
            dt.Columns.Add(new DataColumn("Efektif Alış", typeof(string)));
            dt.Columns.Add(new DataColumn("Efektif Satış", typeof(string)));

            using (XmlTextReader reader = new XmlTextReader(url))
            {
                XmlDocument myxml = new XmlDocument();

                myxml.Load(reader);

                XmlNode tarih = myxml.SelectSingleNode("/ Tarih_Date / @Tarih");
                XmlNodeList mylist = myxml.SelectNodes("/ Tarih_Date / Currency");
                XmlNodeList dovizAdi = myxml.SelectNodes("/ Tarih_Date / Currency / Isim");
                XmlNodeList dovizKodu = myxml.SelectNodes("/ Tarih_Date / Currency / @Kod");
                XmlNodeList birim = myxml.SelectNodes("/ Tarih_Date / Currency / Unit");
                XmlNodeList doviz_alis = myxml.SelectNodes("/ Tarih_Date / Currency / ForexBuying");
                XmlNodeList doviz_satis = myxml.SelectNodes("/ Tarih_Date / Currency / ForexSelling");
                XmlNodeList efektif_alis = myxml.SelectNodes("/ Tarih_Date / Currency / BanknoteBuying");
                XmlNodeList efektif_satis = myxml.SelectNodes("/ Tarih_Date / Currency / BanknoteSelling");

                int x = dovizAdi.Count - 1;

                for (int i = 0; i < x; i++)
                {
                    dr = dt.NewRow();
                    dr[0] = dovizAdi.Item(i).InnerText.ToString();

                    dr[1] = dovizKodu.Item(i).InnerText.ToString();

                    dr[2] = birim.Item(i).InnerText.ToString();

                    decimal dovizAlisValue;
                    if (decimal.TryParse(doviz_alis[i].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out dovizAlisValue))
                    {
                        dr[3] = dovizAlisValue.ToString("0.00", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dr[3] = "0.00"; // Hata durumunda varsayılan değer
                    }

                    decimal dovizSatisValue;
                    if (decimal.TryParse(doviz_satis[i].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out dovizSatisValue))
                    {
                        dr[4] = dovizSatisValue.ToString("0.00", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dr[4] = "0.00"; // Hata durumunda varsayılan değer
                    }

                    decimal efektifAlisValue;
                    if (decimal.TryParse(efektif_alis[i].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out efektifAlisValue))
                    {
                        dr[5] = efektifAlisValue.ToString("0.00", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dr[5] = "0.00"; // Hata durumunda varsayılan değer
                    }

                    decimal efektifSatisValue;
                    if (decimal.TryParse(efektif_satis[i].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out efektifSatisValue))
                    {
                        dr[6] = efektifSatisValue.ToString("0.00", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dr[6] = "0.00"; // Hata durumunda varsayılan değer
                    }

                    dt.Rows.Add(dr);
                }

                reader.Close();

            }
            return dt;
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 != 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
            }
        }
  
    }
}
