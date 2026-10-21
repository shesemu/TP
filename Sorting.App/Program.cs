using Sorting;
internal static class Program {
    [STAThread] static void Main() {
        ApplicationConfiguration.Initialize();
        var form=new Form{Text="Сортировка массива — вариант 3",Width=720,Height=330};
        var path=new TextBox{Left=20,Top=45,Width=550,Text="numbers.txt"};
        var browse=new Button{Left=580,Top=43,Width=100,Text="Выбрать"};
        var run=new Button{Left=20,Top=90,Width=150,Height=35,Text="Сортировать"};
        var result=new TextBox{Left=20,Top=150,Width=660,Height=110,Multiline=true,ReadOnly=true};
        form.Controls.AddRange([new Label{Left=20,Top=20,Width=400,Text="Файл целых чисел, разделённых пробелами"},path,browse,run,result]);
        browse.Click+=(_,_)=>{using var d=new OpenFileDialog();if(d.ShowDialog()==DialogResult.OK)path.Text=d.FileName;};
        run.Click+=(_,_)=>{
            try { result.Text=string.Join(" ",new SortService(new FileArrayReader()).SortFile(path.Text)); }
            catch(Exception e) when(e is IOException or FormatException or OverflowException or ArgumentException or UnauthorizedAccessException) { result.Text="Ошибка: "+e.Message; }
        };
        Application.Run(form);
    }
}
