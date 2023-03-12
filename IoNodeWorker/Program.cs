using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Diagnostics;

namespace IoNodeWorker
{
    static class Program
    {
        /// <summary>
        /// Флаг для работы сборщика муссора
        /// </summary>
        private static bool RunGC = true;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)  // -h -i
        {
            // Проверка на то чтобы не запускался ещё один экземпляр нашего приложения
            if (Process.GetProcesses().Count(x => x.ProcessName == Process.GetCurrentProcess().ProcessName) > 1)
            {
                MessageBox.Show(String.Format("Приложение с именем {0}, уже работает на этом компьютере.", Process.GetCurrentProcess().ProcessName));
                Process.GetCurrentProcess().Kill();
            }

            bool Ishelp = false;
            //string AutoStart = null;
            bool IsInterfase = true;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == @"-h" || args[i] == @"-H" || args[i] == @"-?" || args[i] == @"\h" || args[i] == @"\H" || args[i] == @"\?" || args[i] == @"/h" || args[i] == @"/H" || args[i] == @"/?") Ishelp = true;
                //if (args[i] == @"-s" || args[i] == @"\s" || args[i] == @"/s" || args[i] == @"-S" || args[i] == @"\S" || args[i] == @"/S") { i++; AutoStart = args[i]; }
                if (args[i] == @"-i" || args[i] == @"\i" || args[i] == @"/i" || args[i] == @"-I" || args[i] == @"\I" || args[i] == @"/I") IsInterfase = false;
            }

            try
            {
                // Проверка, если пользователь вызвал справку, то запускать прогу не надо
                if (Ishelp)
                {
                    //Console.WriteLine(@"-s ShortName (Name Task Auto Run)");
                    Console.WriteLine(@"-i (not run interface)");
                }
                else
                {
                    // Проверка по процессам, чтобы приложение было в единственном экземпляре.
                    bool oneOnlyProg;
                    Mutex m = new Mutex(true, Application.ProductName, out oneOnlyProg);
                    if (oneOnlyProg == true /*|| AutoStart != null*/)       // Если это автоматический запуск то можно запускать несколько экземпляров нашего приложения
                    {
                        // Асинхронный запуск процесса
                        Thread thr = new Thread(GarbColRun);
                        //thr = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                        thr.Name = "Thr_GC";
                        thr.IsBackground = true;
                        thr.Start();

                        // Инициализвция классов
                        Com.Log Log = new Com.Log("IoNodeWorker.log");
                        Com.Config Conf = new Com.Config("IoNodeWorker.xml");
                        
                        // Запускаем создание пулов и запускаем в них системные процессы
                        BLL.IoFarm.CreateCurentPulList();

                        /*

                        // создаём просто универсальный класс которым будет управлять ядро оно будет дёргать нужные методы которые идентичны для всех видов благина
                        BLL.Io IoTmp = BLL.IoFarm.CreateIo("Provider");
                        string d1 = IoTmp.CustomClassTyp;

                        // создаём определённый тип плагинов тоже со своим интерфейсом и который тоже будет дёргаться ядром так как он у нас унаследован от универсального плагина то можно использовать полиморфизм и использовать в обычном списке
                        BLL.IoPlg.Provider Prv = BLL.IoPlg.ProviderFarm.CreateProvider("MsSqlPrv");
                        string d2 = Prv.CustomClassTyp;
                        IoTmp = Prv;

                        // При этом на уровне универсального плагина этот плогин типа провайдер и его можно обратно конвертировать в наш под плагин
                        string d3 = IoTmp.CustomClassTyp;
                        // А преобразова в наш кастомный плагин можно понять что это не просто провайдер а провайдер MsSql. 
                        string d4 = ((BLL.IoPlg.Provider)IoTmp).CustomClassTyp;
                        // Таким образом в дальнейшем пожно преобразовать уже в тип провайдер  Ms Sql
                        BLL.IoPlg.ProviderPlg.MsSqlPrv Mstmp = (BLL.IoPlg.ProviderPlg.MsSqlPrv)IoTmp;

                        // Та же логика и спулами провайдеров. Но там скорее всего вложения не будет но если будет то сделано по аналогии с обычными плагинами созданы интерфейсы для того чтобы базовый объект мог дёргать методы из пула объектов. Например по расписанию заставлять плагинные буды отправлять статус или состояние пула для мониторинга итд итп
                        BLL.IoList dd =  BLL.IoFarm.CreatePulIo("ProviderList");
                        string ddfs = dd.CustomClassTyp;
                        

                        BLL.IoList dd =  BLL.IoFarm.CreatePulIo("ProviderList");
                        string ddfs = dd.CustomClassTyp;
                        */

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        // Проверка режима с интерфейсом или нет
                        if (IsInterfase)
                        {
                            Application.Run(new FStart());
                        }
                        else
                        {

                        }

                        // Даём команды на остановку асинхронных процессов
                        BLL.IoFarm.Stop();                  // Во все пулы которые у нас существуют
                        RunGC = false;                      // Сборщику мусора


                        // Закрываем и ждём завершения GB
                        thr.Join();
                        BLL.IoFarm.Join(false);             // Мягкое хавершение процессов дав доработать по текущим заданиям
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (!Ishelp) Com.Log.EventSave("Упали с ошибкой: " + ex.Message, "Main", Lib.EventEn.FatalError);
            }
            finally
            {
                if (!Ishelp) Com.Log.EventSave("Завершили работу программы.", "Main", Lib.EventEn.Message);
            }
        }

        /// <summary>
        /// Асинхронный процесс сборщика мусора
        /// </summary>
        private static void GarbColRun()
        {
            int DefaultCountSec = 60 * 60;
            int CurCountSec = DefaultCountSec;
            while (RunGC)
            {
                if (CurCountSec > 0) { Thread.Sleep(1000); CurCountSec--; }
                else
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    CurCountSec = DefaultCountSec;
                }
            }
        }
    }
}
