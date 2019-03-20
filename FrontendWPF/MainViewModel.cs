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
using System.Data;

namespace FrontendWPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IEnumerable<Employee> employees;
        private DataView table;
        private DataRowView currentRow;
        private Employee currentEmployee;
        
        public Employee CurrentEmployee
        {
            get => currentEmployee;
            set
            {
                currentEmployee = value;
                OnPropertyChanged(nameof(CurrentEmployee));
            }
        }

        public DataView Table
        {
            get => table;
            set
            {
                table = value;
                OnPropertyChanged(nameof(Table));
            }
        }

        public DataRowView CurrentRow
        {
            get => currentRow;
            set
            {
                currentRow = value;
                OnPropertyChanged(nameof(CurrentRow));
                if (currentRow != null)
                    CurrentEmployee = employees.FirstOrDefault(e => e.Name == currentRow.Row.ItemArray.ElementAt(0) as string);
            }
        }

        public MainViewModel()
        {
            bool success = false;
            employees = CallWebAPi<IEnumerable<Employee>>(new Uri("http://localhost:52909"), "api/values", out success);

            Table = CreateDataView(employees);
            
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

        private DataView CreateDataView(IEnumerable<Employee> employees)
        {
            DataTable dataTable = new DataTable();

            // Вычисляю текущее маскимальное количество отпусков.
            int maxVacationNumber = 0;
            foreach (Employee emp in employees)
            {
                if (maxVacationNumber < emp.Vacations.Count)
                    maxVacationNumber = emp.Vacations.Count;
            }

            // Создаю пустые столбцы с заголовками.

            dataTable.Columns.Add("ФИО");

            for (int i = 1; i <= maxVacationNumber * 3; i++)
            {
                switch (i % 3)
                {
                    case 1:
                        dataTable.Columns.Add($"Дата начала {i / 3 + 1}");
                        break;
                    case 2:
                        dataTable.Columns.Add($"Продолжительность {i / 3 + 1}, дней");
                        break;
                    case 0:
                        dataTable.Columns.Add($"Дата окончания {i / 3}");
                        break;
                }
            }

            // Создаю строки, "разбитые" по ячейкам
            foreach (Employee emp in employees)
            {
                string[] myRow = new string[maxVacationNumber * 3 + 1];
                myRow[0] = emp.Name.ToString();
                for (int i = 1; i <= emp.Vacations.Count * 3; i++)
                {
                    switch (i % 3)
                    {
                        case 1:
                            myRow[i] = emp.Vacations[i / 3].Start.ToShortDateString();
                            break;
                        case 2:
                            myRow[i] = emp.Vacations[i / 3].Duration.ToString();
                            break;
                        case 0:
                            myRow[i] = DateTime.Parse(myRow[i - 2]).AddDays(int.Parse(myRow[i - 1])).ToShortDateString();
                            break;
                    }
                }
                dataTable.Rows.Add(myRow);
            }

            return dataTable.DefaultView;
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
