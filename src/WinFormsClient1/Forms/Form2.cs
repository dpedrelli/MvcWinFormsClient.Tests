using Models;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsClient1.Forms
{
    public partial class Form2 : BaseForm
    {
        public Form2()
        {
            InitializeComponent();
        }

        private async Task TestAntiForgeryToken()
        {
            HttpResponseMessage response = await ApiClient.PostAsync("Home/TestAntiForgeryToken", GenerateContent("", ""));
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Success");
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }

        private async Task<List<Product>> GetProducts()
        {
            List<Product> products = null;
            HttpResponseMessage response = await ApiClient.PostAsync("Home/GetProducts", GenerateContent("", ""));
            if (response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<List<Product>>();
            }
            return products;
        }

        private async void button1_Click(object sender, System.EventArgs e)
        {
            await Login(true);
        }

        private async void button3_Click(object sender, System.EventArgs e)
        {
            await TestAntiForgeryToken();
        }

        private async void button2_Click(object sender, System.EventArgs e)
        {
            await GetAntiforgeryToken();
        }

        private async void button4_Click(object sender, System.EventArgs e)
        {
            var products = await GetProducts();
            if (products == null) { return; }
            DataTable dtProducts = new DataTable();
            BindData<Product>(products, dtProducts);
        }
    }
}
