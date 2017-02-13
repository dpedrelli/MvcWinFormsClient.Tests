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
            //bsProducts.DataSource = dtProducts;
            //dataGridView1.DataSource = dtProducts;
            //dataGridView1.DataSource = products;
            //products.ListChanged += delegate (object sender, ListChangedEventArgs args)
            //{
            //    OnListChanged(sender, args);
            //};
            //products.AddingNew += delegate (object sender, AddingNewEventArgs e)
            //{
            //    OnAddingNew(sender, e);
            //};
            //products.ListChanged += OnListChanged;
            //products.AddingNew += OnAddingNew;
            //bsProducts.DataSource = products;
            dataGridView1.DataSource = bsProducts;
        }

        //private void OnListChanged(object sender, ListChangedEventArgs e)
        //{
        //    bsProducts.DataSource = products;
        //}
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

        private void RefreshProducts()
        {
            ////BindData<Product>(products, dtProducts);
            ////dataGridView1.DataSource = bsProducts;
            bsProducts.DataSource = products;
        }
        private async Task/*<List<Product>>*/ GetProducts(string id)
        {
            //List<Product> products = null;
            HttpResponseMessage response = await ApiClient.PostAsync("Home/GetProducts", GenerateContent(id, "id"));
            if (response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<BindingList<Product>>();
                RefreshProducts();
                SetProductDataBindings();
                //BindData<Product>(products, dtProducts);
            }
            //return products;
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
            //var temp = JsonConvert.SerializeObject(product);
            //var temp = new StringContent(new JavaScriptSerializer().Serialize(product), Encoding.UTF8, "application/json");
            var temp = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await ApiClient.PostAsync("Home/DeleteProduct", GenerateContent(await temp.ReadAsStringAsync(), "product"));
            //HttpResponseMessage response = await ApiClient.PostAsJsonAsync("Home/DeleteProduct", product);
            response.EnsureSuccessStatusCode();
            //HttpResponseMessage response = await ApiClient.PostAsync("Home/DeleteProduct", GenerateContent(, "product"));
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
            //products = await GetProducts("");
            //if (products == null) { return; }
            //DataTable dtProducts = new DataTable();
            //BindData<Product>(products, dtProducts);
            //dataGridView1.DataSource = products;
        }

        private void productsBindingNavigatorSaveItem_Click(object sender, System.EventArgs e)
        {

        }

        private void productsBindingSource_CurrentChanged(object sender, System.EventArgs e)
        {

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
                RefreshProducts();
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
