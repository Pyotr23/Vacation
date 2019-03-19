using FrontendWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace FrontendWPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Employee> employees;

        public ObservableCollection<Employee> Employees
        {
            get => employees;
            set
            {
                employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        public MainViewModel()
        {
            bool success = false;
            Employees = new ObservableCollection<Employee>(
                CallWebAPi<IEnumerable<Employee>>(new Uri("http://localhost:52909"), "api/values", out success)); 

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost:52909");

            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));

            //HttpResponseMessage response = client.GetAsync("api/values").Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    Employees = response.Content.ReadAsAsync &
            //    lt; IEnumerable & lt; Employee & gt; &gt; ().Result
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public T CallWebAPi<T>(Uri url, string method, out bool isSuccessStatusCode)
        {
            T result = default(T);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                //    System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password))));

                HttpResponseMessage response = client.GetAsync(method).Result;
                isSuccessStatusCode = response.IsSuccessStatusCode;
                var javaScriptSerializer = new JavaScriptSerializer();
                if (isSuccessStatusCode)
                {
                    var dataobj = response.Content.ReadAsStringAsync();
                    result = javaScriptSerializer.Deserialize<T>(dataobj.Result);
                }
                else if (Convert.ToString(response.StatusCode) != "InternalServerError")
                {
                    result = javaScriptSerializer.Deserialize<T>("{ \"APIMessage\":\"" + response.ReasonPhrase + "\" }");
                }
                else
                {
                    result = javaScriptSerializer.Deserialize<T>("{ \"APIMessage\":\"InternalServerError\" }");
                }
            }
            return result;
        }
    }
}
