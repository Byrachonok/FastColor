using System.Windows.Forms;
using System.Drawing;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using CADapp = Autodesk.AutoCAD.ApplicationServices.Application;
using CADdoc = Autodesk.AutoCAD.ApplicationServices.Document;

namespace Plugin
{
    //UserColor отвечает за выбранный пользователем цвет
    //Вынос в статический класс позволяет взаимодействовать с ним в любой части кода
    public static class UserColor
    {
        //По умолчанию цвет устанавливается белым
        private static Color ThisColor = Color.White;

        //Свойство возвращает текущий цвет, позволяет задать пользовательский
        public static Color currentColor
        {
            get { return ThisColor; }
            set { ThisColor = value; }
        }
    }

    //Собственно класс самого плагина
    public class Plugin
    {
        #region //Вызов пользовательского диалогового окна настроек плагина
        [CommandMethod("OptionsFastColor")]
        public void Options()
        {
            //Создание окна 
            OptionsWinForm.MainWindow WinForm = new OptionsWinForm.MainWindow();
            DialogResult AnswerDialog = CADapp.ShowModalDialog(WinForm);

            //Проверка результата диалога
            if (AnswerDialog != DialogResult.OK) return;

            //Перенос результатов из формы в тело плагина
            UserColor.currentColor = WinForm.ResultColor;
        }
        #endregion

        #region  //Метод быстрой замены цвета выбранных обьектов 
        [CommandMethod("F")]
        public void Paint()
        {
            //Получение базы и редактора текущего документа
            CADdoc Documetn = CADapp.DocumentManager.MdiActiveDocument;
            Database DocDataBase = Documetn.Database;
            Editor DocEditor = Documetn.Editor;

            //Получение ссылки на выделенный объект
            PromptSelectionResult SelectObj = DocEditor.GetSelection();

            //Если выбор завершился без успеха
            if (SelectObj.Status != PromptStatus.OK)
            {
                DocEditor.WriteMessage("Ошибка при выборе объекта!");
                return;
            }

            //Ids - массив с ID выбранных ранее объектов
            ObjectId[] Ids = SelectObj.Value.GetObjectIds();

            //Транзакция
            using (Transaction t = DocDataBase.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in Ids)
                {
                    //Приведение каждого выбранного раннее обьекта к Entity
                    Entity entity = (Entity)t.GetObject(id, OpenMode.ForRead);

                    try //Попытка получения доступа к объекту
                    {
                        //Открытие объекта на запись
                        entity.UpgradeOpen(); //GetObject

                        //разложение объекта UserColor на 3 компонента и применение их объектам AutoCAD
                        entity.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(
                            UserColor.currentColor.R,   //Красный компонент цвета
                            UserColor.currentColor.G,   //Зеленый компонент
                            UserColor.currentColor.B);  //Синий компонент
                    }
                    catch
                    {
                        DocEditor.WriteMessage("При попытке получения доступа к объекту произошла ошибка");
                    }
                }
                t.Commit();
            }
        }
        #endregion
    }
}
