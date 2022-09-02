using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Bitez_Dondurma___Dondurma_Sipariş_Uygulaması
{
    public partial class SiparisForm : Form
    {
        SqlConnection baglanti;
        SqlCommand komut;
        SqlDataAdapter da;
        public SiparisForm()
        {
            InitializeComponent();
        }

        void SiparisGetir()
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-U9FK9EG; Initial Catalog=bitezdondurma; Persist Security Info=False; User ID=sa; Password=1234");
            baglanti.Open();
            da = new SqlDataAdapter("SELECT *FROM siparis", baglanti);
            DataTable tablo = new DataTable();
            da.Fill(tablo);
            dataGridView1.DataSource = tablo;
            baglanti.Close();
        }

        private void baslikGoster()
        {
            dataGridView1.Columns[0].HeaderText = "Sipariş No";
            dataGridView1.Columns[0].Width = 80;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].HeaderText = "Çeşit";
            dataGridView1.Columns[1].Width = 80;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].HeaderText = "Boyut";
            dataGridView1.Columns[2].Width = 60;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderText = "Adet";
            dataGridView1.Columns[3].Width = 80;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].HeaderText = "Sipariş Tarihi";
            dataGridView1.Columns[4].Width = 120;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
        }

        private void SiparisForm_Load(object sender, EventArgs e)
        {
            SiparisGetir();
            baslikGoster();
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtNo.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtCesit.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            txtBoyut.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            txtAdet.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            dateTimePicker1.Text=dataGridView1.CurrentRow.Cells[4].Value.ToString();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string sorgu = "INSERT INTO siparis (cesit,boyut,adet,dtarih) VALUES (@cesit,@boyut,@adet,@dtarih)";
            komut = new SqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@cesit", txtCesit.Text);
            komut.Parameters.AddWithValue("@boyut", txtBoyut.Text);
            komut.Parameters.AddWithValue("@adet", txtAdet.Text);
            komut.Parameters.AddWithValue("@dtarih", dateTimePicker1.Value);
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
            SiparisGetir();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            string sorgu = "DELETE FROM siparis WHERE mno=@mno";
            komut = new SqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@mno", Convert.ToInt32(txtNo.Text));
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
            SiparisGetir();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            string sorgu="UPDATE siparis SET cesit=@cesit,boyut=@boyut,adet=@adet,dtarih=@dtarih WHERE mno=@mno";
            komut = new SqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@mno", Convert.ToInt32(txtNo.Text));
            komut.Parameters.AddWithValue("@cesit", txtCesit.Text);
            komut.Parameters.AddWithValue("@boyut", txtBoyut.Text);
            komut.Parameters.AddWithValue("@adet", txtAdet.Text);
            komut.Parameters.AddWithValue("@dtarih", dateTimePicker1.Value);
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
            SiparisGetir();

        }

        private void btnGonder_Click(object sender, EventArgs e)
        {
            if(txtMail.Text=="")
            {
                MessageBox.Show("Lütfen Mail Adresi Belirtiniz");
            }
            else
            {
                string mailBody = "<table width='100%' style='border':Solid 1px Black'>";

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    mailBody += "<tr>";
                    foreach (DataGridViewCell cell  in row.Cells)
                    {
                        mailBody += "<td style ='color:blue;'>" + cell.Value + "</td>";
                    }
                    mailBody += "</tr>";
                }
                mailBody += "</table>";

                MailMessage msj = new MailMessage();
                msj.To.Add(txtMail.Text);
                msj.From = new MailAddress("siparisbitezdondurma@gmail.com", "GÜMÜŞLÜK SİPARİŞ");
                msj.Subject = ("Gümüşlük Bitez Dondurma Güncel Sipariş");
                msj.IsBodyHtml = true;
                msj.Body = mailBody;

                SmtpClient Client = new SmtpClient();
                Client.UseDefaultCredentials = true;
                Client.Credentials = new System.Net.NetworkCredential("siparisbitezdondurma@gmail.com", "emrecan123");
                Client.Host = "smtp.gmail.com";
                Client.Port = 587;
                Client.EnableSsl = true;
                Client.Send(msj);
                msj.Dispose();
                MessageBox.Show("Siparişiniz Gönderildi");
                
            }
        }
    }
}
