using Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsClient1;
using System;

namespace WinFormsClient1.Forms
{
    public partial class Form2 : BaseForm
    {
        private BindingList<Product> products = new BindingList<Product>();
        //private DataTable dtProducts = new DataTable();
        private BindingSource bsProducts = new BindingSource();

        public Form2()
        {
            InitializeComponent();
            products.ListChanged += OnListChanged;
            //products.AddingNew += OnAddingNew;
            bsProducts.DataSource = products;
            dataGridView1.DataSource = bsProducts;
        }

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            bsProducts.DataSource = products;
        }
        //private void OnAddingNew(object sender, AddingNewEventArgs e)
        //{
        //    bsProducts.DataSource = products;
        //}

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

        private async Task GetProducts(string id)
        {
            HttpResponseMessage response = await ApiClient.PostAsync("Home/GetProducts", GenerateContent(id, "id"));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<List<Product>>();
                products.Clear();
                foreach (var item in result)
                {
                    products.Add(item);
                }
                SetProductDataBindings();
            }
        }

        bool _productBound = false;
        private void SetProductDataBindings()
        {
            if (_productBound) { return; }
            edtId.DataBindings.Add("Text", bsProducts, "Id");
            _productBound = true;
        }

        private async Task DeleteProduct(Product product)
        {
            //var temp = new StringContent(new JavaScriptSerializer().Serialize(product), Encoding.UTF8, "application/json");
            var temp = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            //HttpResponseMessage response = await ApiClient.PostAsync("Home/DeleteProduct", GenerateContent(await temp.ReadAsStringAsync(), "product"));
            HttpResponseMessage response = await ApiClient.PostAsJsonAsync("Home/DeleteProduct", product);
            response.EnsureSuccessStatusCode();
        }
        private async Task<bool> DeleteProductAsString(Product product)
        {
            var temp = JsonConvert.SerializeObject(product);
            HttpResponseMessage response = await ApiClient.PostAsync("Home/DeleteProductAsString", GenerateContent(temp, "product"));
            return (response.StatusCode == HttpStatusCode.OK);
        }

        private async Task UpdateProduct(Product product)
        {
            HttpResponseMessage response = await ApiClient.PostAsJsonAsync("Home/UpdateProduct", product);
            response.EnsureSuccessStatusCode();
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
            await GetProducts("");
        }

        private async void button5_Click(object sender, System.EventArgs e)
        {
            await GetProducts(txtProductId.Text);
        }

        private async void button6_Click(object sender, System.EventArgs e)
        {
            if (await DeleteProductAsString(products[dataGridView1.CurrentCell.RowIndex]))
            {
                products.Remove(products[dataGridView1.CurrentCell.RowIndex]);
                return;
            }
            MessageBox.Show("Delete Failed");
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtProductId.Text = products[dataGridView1.CurrentCell.RowIndex].Id;
        }

    }
}
