﻿using FrontendWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Data;
using System.Windows;
using System.Collections;
using System.Threading.Tasks;

namespace FrontendWPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static IEnumerable<Employee> employees;
        private DataView table;
        private DataRowView currentRow;
        private string name;
        private string empColor;
        private static HttpClient client = new HttpClient();
        private Employee currentEmployee;
        private IEnumerable<Vacation> vacations;
        private DateTime start = DateTime.Now;
        private string duration;
        private Vacation currentVacation;
        //private List<Cell[]> cellRows = new List<Cell[]>();
        private Cell[,] cells;

        public RelayCommand AddEmployee { get; set; }
        public RelayCommand DeleteEmployee { get; set; }
        public RelayCommand CommandAddVacation { get; set; }
        public RelayCommand CommandDeleteVacation { get; set; }  
        
        public IEnumerable<string> EmployeeNames { get; set; }

        public Cell[,] Cells
        {
            get => cells;
            set
            {
                cells = value;
                OnPropertyChanged(nameof(Cells));
            }
        }

        public Cell[,] FirstQuarter { get; set; }
        public Cell[,] SecondQuarter { get; set; }
        public Cell[,] ThirdQuarter { get; set; }
        public Cell[,] FourthQuarter { get; set; }

        //public List<Cell[]> CellRows
        //{
        //    get => cellRows;
        //    set
        //    {
        //        cellRows = value;
        //        OnPropertyChanged(nameof(CellRows));
        //    }
        //}

        public Vacation CurrentVacation
        {
            get => currentVacation;
            set
            {
                currentVacation = value;
                OnPropertyChanged(nameof(CurrentVacation));
                if (currentVacation != null)
                {
                    Start = currentVacation.Start;
                    Duration = currentVacation.Duration.ToString();
                }                
            }
        }

        public string Duration
        {
            get => duration;
            set
            {
                duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        public DateTime Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        public IEnumerable<Vacation> Vacations
        {
            get => vacations;
            set
            {
                vacations = value;
                OnPropertyChanged(nameof(Vacations));
            }
        }

        public string[] Colors { get; set; } = 
            {
                "Red",
                "Orange",
                "Yellow",
                "Green",
                "Azure",
                "Blue",
                "Purple"
            };        

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string EmpColor
        {
            get => empColor;
            set
            {
                empColor = value;
                OnPropertyChanged(nameof(empColor));
            }
        }

        public DataView Table
        {
            get => table;
            set
            {
                table = value;                             
                OnPropertyChanged(nameof(Table));
                EmployeeNames = employees.Select(x => x.Name).ToList();
                OnPropertyChanged(nameof(EmployeeNames));
                CreateDataSecondTable(employees);
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
                {
                    currentEmployee = employees.FirstOrDefault(e => e.Name == currentRow.Row.ItemArray.ElementAt(0) as string);
                    Name = currentEmployee.Name;
                    EmpColor = currentEmployee.Color;
                    Vacations = currentEmployee.Vacations; 
                }                    
            }
        }

        public MainViewModel()
        {
            //CreateDataSecondTable(employees);
            //CellRows.Add(new Cell[] { new Cell("Red"), new Cell("Green") });
            //CellRows.Add(new Cell[] { new Cell("Yellow"), new Cell("Red") });
            //OnPropertyChanged(nameof(CellRows));

            client.BaseAddress = new Uri("http://localhost:52909");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GetAllEmployees();             

            AddEmployee = new RelayCommand(o => NewEmployee(), v => EmpColor != null);
            DeleteEmployee = new RelayCommand(o => 
            {
                DelEmployee();
                Name = "";
                EmpColor = null;
            }, 
            o => currentEmployee != null && currentEmployee.Name == this.Name);

            CommandAddVacation = new RelayCommand(o => AddVacation());
            CommandDeleteVacation = new RelayCommand(o => DeleteVacation());
            
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

        private async void AddVacation()
        {
            try
            {
                Vacation newVacation = new Vacation() { Start = this.Start, Duration = int.Parse(this.Duration) };
                var response = await client.PostAsJsonAsync("/api/values/vacation/", 
                    new VacationViewModel() { EmployeeId = currentEmployee.EmployeeId, Vacation = newVacation });
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                //MessageBox.Show("Student Added Successfully", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                //studentsListView.ItemsSource = await GetAllStudents();
                //studentsListView.ScrollIntoView(studentsListView.ItemContainerGenerator.Items[studentsListView.Items.Count - 1]);
                GetAllEmployees();                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Student not Added, May be due to Duplicate ID");
            }
        }

        public async void DeleteVacation()
        {
            try
            {
                var response = await client.DeleteAsync($"/api/values/{currentEmployee.EmployeeId}/{currentVacation.VacationId}");
                response.EnsureSuccessStatusCode();
                GetAllEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Student not Added, May be due to Duplicate ID");
            }
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

        public async void GetAllEmployees()
        {            
            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/values/");
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();
                Table = CreateDataView(employees);
                //await Task.Factory.StartNew(() =>
                // {
                //     var rtyu = true;
                //     while (rtyu)
                //     {
                //         if (employees != null)
                //         {
                //             CreateDataSecondTable(employees);
                //             rtyu = false;
                //         }
                //     }
                // });

                //CreateDataSecondTable(employees);
            }
            catch (Exception)
            {
                //MessageBox.Show("Error!");
            }            
        }

        public void CreateDataSecondTable(IEnumerable<Employee> data)
        {
            int firstQuarterEnd = DateTime.Parse("31/03/2019").DayOfYear;
            FirstQuarter = new Cell[data.Count(), firstQuarterEnd];
            int secondQuarterEnd = DateTime.Parse("30/06/2019").DayOfYear;
            SecondQuarter = new Cell[data.Count(), secondQuarterEnd - firstQuarterEnd];
            int thirdQuarterEnd = DateTime.Parse("30/09/2019").DayOfYear;
            ThirdQuarter = new Cell[data.Count(), thirdQuarterEnd - secondQuarterEnd];
            int fourthQuarterEnd = DateTime.Parse("31/12/2019").DayOfYear;
            FourthQuarter = new Cell[data.Count(), fourthQuarterEnd - thirdQuarterEnd];
            Vacation vacation;
            HashSet<int> ht = new HashSet<int>();
            Cell backCell = new Cell("AntiqueWhite");
            Cell frontCell;

            for (int n = 0; n < data.Count(); n++)
            {
                frontCell = new Cell(data.ElementAt(n).Color);

                for (int i = 0; i < data.ElementAt(n).Vacations.Count; i++)
                {
                    vacation = data.ElementAt(n).Vacations[i];
                    for (int j = 0; j < vacation.Duration; j++)
                    {
                        ht.Add(vacation.Start.DayOfYear + j);
                    }
                }                

                for (int i = 0; i < firstQuarterEnd; i++)
                {
                    FirstQuarter[n, i] = ht.Contains(i) ? frontCell : backCell;
                }

                for (int i = firstQuarterEnd; i < secondQuarterEnd; i++)
                {
                    SecondQuarter[n, i - firstQuarterEnd] = ht.Contains(i) ? frontCell : backCell;
                }

                for (int i = secondQuarterEnd; i < thirdQuarterEnd; i++)
                {
                    ThirdQuarter[n, i - secondQuarterEnd] = ht.Contains(i) ? frontCell : backCell;
                }

                for (int i = thirdQuarterEnd; i < fourthQuarterEnd; i++)
                {
                    FourthQuarter[n, i - thirdQuarterEnd] = ht.Contains(i) ? frontCell : backCell;
                }

                ht.Clear();                
            }
            OnPropertyChanged(nameof(FirstQuarter));
            OnPropertyChanged(nameof(SecondQuarter));
            OnPropertyChanged(nameof(ThirdQuarter));
            OnPropertyChanged(nameof(FourthQuarter));
        }


        //public void CreateDataSecondTable(IEnumerable<Employee> data)
        //{
        //    Cells = new Cell[data.Count(), 365];
        //    //CellRows.Capacity = data.Count();
        //    Vacation vacation;
        //    HashSet<int> ht = new HashSet<int>();
        //    Cell backCell = new Cell("AntiqueWhite");
        //    Cell frontCell;

        //    for (int n = 0; n < data.Count(); n++)
        //    {                
        //        for (int i = 0; i < data.ElementAt(n).Vacations.Count; i++)
        //        {
        //            vacation = data.ElementAt(n).Vacations[i];
        //            for (int j = 0; j < vacation.Duration; j++)
        //            {
        //                ht.Add(vacation.Start.DayOfYear + j);
        //            }
        //        }
                
        //        frontCell = new Cell(data.ElementAt(n).Color);
        //        for (int i = 0; i < 365; i++)
        //        {
        //            Cells[n, i] = ht.Contains(i) ? frontCell : backCell;                    
        //        }
        //        ht.Clear();                
        //        OnPropertyChanged(nameof(Cells));
        //    }
        //}


        //public void CreateDataSecondTable(IEnumerable<Employee> data)
        //{
        //    if (data != null)
        //    {
        //        for (int i = 0; i < data.Count(); i++)
        //        {
        //            CellRows.Add(new Cell[365]);
        //        }
        //        OnPropertyChanged(nameof(CellRows));

        //        CellRows.Clear();
        //        CellRows.Capacity = data.Count();
        //        Vacation vacation;
        //        HashSet<int> ht = new HashSet<int>();
        //        Cell backCell = new Cell("AntiqueWhite");
        //        Cell frontCell;

        //        for (int n = 0; n < data.Count(); n++)
        //        {
        //            CellRows.Add(new Cell[365]);
        //            for (int i = 0; i < data.ElementAt(n).Vacations.Count; i++)
        //            {
        //                vacation = data.ElementAt(n).Vacations[i];
        //                for (int j = 0; j < vacation.Duration; j++)
        //                {
        //                    ht.Add(vacation.Start.DayOfYear + j);
        //                }
        //            }
        //            Cell[] cellsInRow = new Cell[365];
        //            frontCell = new Cell(data.ElementAt(n).Color);
        //            for (int i = 0; i < 365; i++)
        //            {
        //                cellsInRow[i] = ht.Contains(i) ? frontCell : backCell;
        //            }
        //            ht.Clear();
        //            CellRows[n] = cellsInRow;
        //            CellRows.Add(cellsInRow);
        //        }
        //    }
        //    else
        //    {
        //        CellRows.Add(new Cell[365]);
        //        CellRows.Add(new Cell[365]);
        //        CellRows.Add(new Cell[365]);
        //        CellRows.Add(new Cell[365]);

        //        CellRows.Add(new Cell[] { new Cell("Red"), new Cell("Green") });
        //        CellRows.Add(new Cell[] { new Cell("Green"), new Cell("Red") });
        //    }
        //    OnPropertyChanged(nameof(CellRows));
        //}

        public async void NewEmployee()
        {
            try
            {
                Employee newEmployee = new Employee() { Name = this.Name, Color = EmpColor };
                var response = await client.PostAsJsonAsync("/api/values/", newEmployee);
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                //MessageBox.Show("Student Added Successfully", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                //studentsListView.ItemsSource = await GetAllStudents();
                //studentsListView.ScrollIntoView(studentsListView.ItemContainerGenerator.Items[studentsListView.Items.Count - 1]);
                GetAllEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Student not Added, May be due to Duplicate ID");
            }
        }

        public async void DelEmployee()
        {
            try
            {
                var response = await client.DeleteAsync($"/api/values/{currentEmployee.EmployeeId}");
                response.EnsureSuccessStatusCode();
                GetAllEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Student not Added, May be due to Duplicate ID");
            }
        }

        //public T CallWebAPi<T>(Uri url, string method, out bool isSuccessStatusCode)
        //{
        //    T result = default(T);

        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = url;
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
        //        //    System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password))));

        //        HttpResponseMessage response = client.GetAsync(method).Result;
        //        isSuccessStatusCode = response.IsSuccessStatusCode;
        //        var javaScriptSerializer = new JavaScriptSerializer();
        //        if (isSuccessStatusCode)
        //        {
        //            var dataobj = response.Content.ReadAsStringAsync();
        //            result = javaScriptSerializer.Deserialize<T>(dataobj.Result);
        //        }
        //        else if (Convert.ToString(response.StatusCode) != "InternalServerError")
        //        {
        //            result = javaScriptSerializer.Deserialize<T>("{ \"APIMessage\":\"" + response.ReasonPhrase + "\" }");
        //        }
        //        else
        //        {
        //            result = javaScriptSerializer.Deserialize<T>("{ \"APIMessage\":\"InternalServerError\" }");
        //        }
        //    }
        //    return result;
        //}
    }    
}
