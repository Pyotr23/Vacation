using FrontendWPF.Models;
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
using System.Text;
using System.Configuration;

namespace FrontendWPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static IEnumerable<Employee> employees;        
        private static HttpClient client = new HttpClient();
        private Employee currentEmployee;                     
        private StringBuilder errorSB = new StringBuilder();

        public RelayCommand AddEmployee { get; set; }
        public RelayCommand DeleteEmployee { get; set; }
        public RelayCommand CommandAddVacation { get; set; }
        public RelayCommand CommandDeleteVacation { get; set; }  
        public RelayCommand CommandRefresh { get; set; }
        
        public Cell[,] FirstQuarter { get; set; }
        public Cell[,] SecondQuarter { get; set; }
        public Cell[,] ThirdQuarter { get; set; }
        public Cell[,] FourthQuarter { get; set; }

        private IEnumerable<string> employeeNames;
        public IEnumerable<string> EmployeeNames
        {
            get => employeeNames;
            set
            {
                employeeNames = value;
                OnPropertyChanged(nameof(EmployeeNames));
            }
        }


        private string error;
        // Свойство для записывания ошибок
        public string Error
        {
            get => error;
            set
            {
                error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        private Cell[,] cells;
        public Cell[,] Cells
        {
            get => cells;
            set
            {
                cells = value;
                OnPropertyChanged(nameof(Cells));
            }
        }       

        private Vacation currentVacation;
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

        private string duration;
        public string Duration
        {
            get => duration;
            set
            {
                duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        private DateTime start = DateTime.Now;
        public DateTime Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        private IEnumerable<Vacation> vacations;
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

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string empColor;
        public string EmpColor
        {
            get => empColor;
            set
            {
                empColor = value;
                OnPropertyChanged(nameof(empColor));
            }
        }

        private DataView table;
        public DataView Table
        {
            get => table;
            set
            {
                table = value;                             
                OnPropertyChanged(nameof(Table));
                EmployeeNames = employees.Select(x => x.Name).ToList();                
                //CreateDataSecondTable(employees);                
            }
        }

        private DataRowView currentRow;
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
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("localhost"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            employees = GetAllEmployees();
            if (employees != null)
            {
                Table = CreateDataView(employees);
                CreateDataSecondTable(employees);
            }            
            Error = errorSB.ToString();

            AddEmployee = new RelayCommand(o => {
                errorSB.Clear();
                currentEmployee = NewEmployee();
                employees = GetAllEmployees();
                if (employees != null)
                    CreateDataSecondTable(employees);
                Table.Table.Rows.Add(currentEmployee.Name);
                EmployeeNames = employees.Select(x => x.Name).ToList();
                //Table.Sort = "ФИО";
                OnPropertyChanged(nameof(Table));               

                //CurrentRow = Table.FindRows(new object[] { currentEmployee.Name })[0];

                Error = errorSB.ToString();
            }, v => EmpColor != null);

            DeleteEmployee = new RelayCommand(o => 
            {
                errorSB.Clear();
                DelEmployee();
                employees = GetAllEmployees();
                if (employees != null)
                {
                    Table = CreateDataView(employees);
                    CreateDataSecondTable(employees);
                }
                Name = "";
                EmpColor = null;
                Error = errorSB.ToString();
            }, 
            o => currentEmployee != null && currentEmployee.Name == this.Name);

            int durationDigit;
            CommandAddVacation = new RelayCommand(o => AddVacation(), v => int.TryParse(Duration, out durationDigit));
            CommandDeleteVacation = new RelayCommand(o => DeleteVacation(), v => CurrentVacation != null);

            CommandRefresh = new RelayCommand(o => {
                errorSB.Clear();
                employees = GetAllEmployees();
                if (employees != null)
                {
                    Table = CreateDataView(employees);
                    CreateDataSecondTable(employees);
                }
                Error = errorSB.ToString();
            });
        }       

        public IEnumerable<Employee> GetAllEmployees()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync("/api/values/").Result;
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                return response.Content.ReadAsAsync<IEnumerable<Employee>>().Result;
            }
            catch (Exception)
            {
                errorSB.AppendLine("Проблема с получением всех сотрудников.");
                return null;
            }
        }

        // Создание первой таблицы на основе коллекции сотрудников.
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
                            myRow[i] = DateTime.Parse(myRow[i - 2]).AddDays(int.Parse(myRow[i - 1]) - 1).ToShortDateString();
                            break;
                    }
                }
                dataTable.Rows.Add(myRow);
            }

            return dataTable.DefaultView;
        }

        // Создание второй таблицы на основе коллекции сотрудников.
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

        public Employee NewEmployee()
        {
            try
            {
                Employee newEmployee = new Employee() { Name = this.Name, Color = EmpColor };
                HttpResponseMessage response = client.PostAsJsonAsync("/api/values/", newEmployee).Result;
                response.EnsureSuccessStatusCode(); // Throw on error code.  
                return response.Content.ReadAsAsync<Employee>().Result;
                //GetAllEmployees();
            }
            catch (Exception)
            {
                errorSB.AppendLine("Проблема с добавлением сотрудника.");
                return null;
            }
        }

        public void DelEmployee()
        {
            try
            {
                var response = client.DeleteAsync($"/api/values/{currentEmployee.EmployeeId}").Result;
                response.EnsureSuccessStatusCode();                
            }
            catch (Exception)
            {
                errorSB.AppendLine("Проблема с удалением сотрудника.");                
            }
        }

        private async void AddVacation()
        {
            try
            {
                Vacation newVacation = new Vacation() { Start = this.Start, Duration = int.Parse(this.Duration) };
                var response = await client.PostAsJsonAsync("/api/values/vacation/", 
                    new VacationViewModel() { EmployeeId = currentEmployee.EmployeeId, Vacation = newVacation });
                response.EnsureSuccessStatusCode();                
                GetAllEmployees();                
            }
            catch (Exception ex)
            {
                
            }
        }

        public async void DeleteVacation()
        {
            try
            {
                var response = await client.DeleteAsync($"/api/values/{currentEmployee.EmployeeId}/{currentVacation.VacationId}");
                response.EnsureSuccessStatusCode();
                GetAllEmployees();
                CurrentVacation = null;
                Duration = "";
            }
            catch (Exception ex)
            {
                
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }    
}
