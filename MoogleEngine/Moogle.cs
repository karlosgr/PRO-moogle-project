namespace MoogleEngine;


public static class Moogle
{
//Main                  //el enfoque de este proyecto fue el trabajo con diccionarios
public static string[] direccion = Directory.GetFiles(@"..\Content", "*txt");//direccion de los txt


public static string[] separator(string query) //   esto es el metodo para separar una string en un array de palabras para q sea mas facil trabajar con el
    {
        char[] separadores = new char[] { ' ', '?', ';', ',', '.', '-', ':', '+', '#', '$', '%', '&', '(', ')', '¿', '?','_','[',']','{','}','<','>' };
        string[] separado = query.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
        return separado;
    }


public static string sugestion = "";


public static Dictionary<string, Dictionary<string, int>> main =new Dictionary<string, Dictionary<string, int>>();// diccionario base donde se guardan los documentos como llave y cada documento tiene asociado un diccionario con sus palabras y la cantidad de veces q se repite

public static Dictionary<string,Dictionary<string,double>> termfrec =new Dictionary<string, Dictionary<string, double>>();//diccionario donde se almacenan las palabras con su tf por documento

public static Dictionary<string, double> invertedfrec =new Dictionary<string, double>();// diccionario donde se almacena los documentos con su score dada la query

public static Dictionary<string, Dictionary<string, List<string>>> cercan =new Dictionary<string, Dictionary<string, List<string>>>();//en este diccionario van los documentos com todas sus palabras cercanas

public static Dictionary<string,double> score = new Dictionary<string,double>();//resultado final

public static int cantidadderesults;

public static int numeroderesultados;//esta variable es para el boton mostrar mas 

public static bool ensenar; //numero de resultados q voy a mostrar












//metodos


 public static SearchResult Query(string query) {



        

query=separadorop(query).ToLower();//con este metodo si los operadores estan unidos a las palabras los separa para poder trabajar mas facil con ellos

string[] forsearch=separator2(query);//en este array se guardan las palabras de la query sin los operadores ya listas para ejecutar la busqueda 
string[] forop=separator(query);// aqui se guardan con los operadores para pasarselo a los metodos q reconocen los operadores y trabajan con ellos




Dictionary<string,string> opicer=operadorcercania(forop);//aqui van los duos de palabras modificados con el operador de cercania

Dictionary<string,string> opapar=operadoresap(forop);// aqui van las palabras asociadas a los operadores ap y nap y q operador es

Dictionary<string,int> opimpor=operadorimportancia(forop);//en este diccionario van las palabras modificadas por asteriscos y la cantidad de asteriscos q tienen





 string separadorop(string input)//con este metodo separo los operadores de busqueda q esten unidos para trabajar mas facil con ellos
{
    string result = input;
    char[] operators = new char[] { '*', '!', '~', '^' };
    foreach (char op in operators)//la operacion se repite por cada operador
    {
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] == op) { result = result.Insert(i, " "); result = result.Insert(i + 2, " "); i++; }//si recorriendo el string me encuentro con un operador de busqueda inserto espacios en blanco delante y despues de el 
        }//usando el metodo insert para poner espacios en blancos

    }

    return result;
}



static string[] separator2(string query)//este metodo es igual q el de arriba pero este si elimina los operadores de busqueda
{
    char[] separadores = new char[] { ' ', '?', ';', ',', '.', '-', ':', '+', '#', '$', '%', '&', '(', ')', '¿', '?', '_', '[', ']', '{', '}', '<', '>','~','^','*','!' };
    string[] separado = query.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
    return separado;
}



static Dictionary<string, int> operadorimportancia(string[] query)//este metodo recive la entrada y devuuelve las palabras modificadas por * y cuantos de estos tiene
{
    Dictionary<string, int> result = new Dictionary<string, int>();//por ahora los voy a almacenar en un diccionario  
    bool encontrado = false;
    int counterasterisco = 0;
    for (int i = 0; i < query.Length; i++)//este ciclo busca los asteriscos
    {

        if (query[i] == "*")//si se encuentra un asterisco añade un valor al contador y el bool de q encontro un astersico es true
        {
            encontrado = true;
            counterasterisco++;

        }
        if (query[i] != "*" && encontrado == true) { result.Add(query[i], counterasterisco); counterasterisco = 0; encontrado = false; }
        //cuando se encuentre un valor distinto del asterisco despues de q ya encontro un astersico pues lo añade al diccionario con el contador

    }

    return result;
}




static Dictionary<string, string> operadoresap(string[] input)//por ahora voy a almacenar las palabras q se encuentran modificadas por este operador en un diccionario
{
    Dictionary<string, string> result = new Dictionary<string, string>();
    string opap = "^";
    string opnap = "!";
    for (int i = 0; i < input.Length; i++)
    {
        if (input[i] == opap || input[i] == opnap) { result.Add(input[i + 1], input[i]); }// si se encuentra alguno de los dos operadores lo almacena en el diccionario con el operador q lo modifica
    }
    return result;
}




static Dictionary<string, string> operadorcercania(string[] input)//con este metodo almaceno en un diccionario las palabras relacionadas con el operador cercania de dos en dos
{
    string opcer = "~";
    Dictionary<string, string> result = new Dictionary<string, string>();
    for (int i = 0; i < input.Length; i++)
    {
        if (input[i] == opcer) { result.Add(input[i - 1], input[i + 1]); }

    }
    return result;
}




static bool soncercanos( Dictionary<string, List<string>> prin,string uno,string dos)
{
    foreach(var dic in prin)
    {
      if((prin.ContainsKey(uno) && dic.Value.Contains(dos)) || prin.ContainsKey(dos) && dic.Value.Contains(uno)) { return true; }

    }
    return false;
}


static string title(string paht)//este metodo es para enviar el titulo de los documentos ya q solo tengo la direccion
{
    string a = @"\";
    char[] b = a.ToCharArray();
    string title = "";
    for(int i = paht.Length - 1; i >= 0; i--)
    {
        if(paht[i] == b[0]) { title = paht.Substring(i+1); break; }

    }

    return title;
}




static int LevenshteinDistance(string s, string t)
{
    // d es una tabla con m+1 renglones y n+1 columnas
    int costo = 0;
    int m = s.Length;
    int n = t.Length;
    int[,] d = new int[m + 1, n + 1];

    // Verifica que exista algo que comparar
    if (n == 0) return m;
    if (m == 0) return n;

    // Llena la primera columna y la primera fila.
    for (int i = 0; i <= m; d[i, 0] = i++) ;
    for (int j = 0; j <= n; d[0, j] = j++) ;


    /// recorre la matriz llenando cada unos de los pesos.
    /// i columnas, j renglones
    for (int i = 1; i <= m; i++)
    {
        // recorre para j
        for (int j = 1; j <= n; j++)
        {
            /// si son iguales en posiciones equidistantes el peso es 0
            /// de lo contrario el peso suma a uno.
            costo = (s[i - 1] == t[j - 1]) ? 0 : 1;
            d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1,  //Eliminacion
                          d[i, j - 1] + 1),                             //Insercion 
                          d[i - 1, j - 1] + costo);                     //Sustitucion
        }
    }

    return d[m, n];
}


static string[] sugerencia(string[] query, Dictionary<string, double> invertedfrec)//este metodo buscara la palabra mas cercana en los documentos si no da suficientes resultados la busqueda
{
    string[] result = new string[query.Length];
    int count = 0;
    foreach (string word in query)//por cada palabra de la busqueda
    {
        if (invertedfrec.ContainsKey(word) == false)//si la palabra no aparece en ningun documento 
        {
            int distance = int.MaxValue;
            foreach (var llave in invertedfrec.Keys)
            {
                distance = Math.Min(LevenshteinDistance(word, llave), distance);//se calcula la distancia minima q tenga con una palabra de los documentos
            }

            foreach (var llave in invertedfrec.Keys) { if (LevenshteinDistance(word, llave) == distance) { result[count] = llave; count++; break; } }

        }
        else { result[count] = word; count++; }


    }
    return result;

}












//busqueda principal
foreach (var dic in termfrec)//aqui la busqueda ya se vuelve mas sencilla por cada documento veo su valor viendo las palabras de la query y buscando su tfidf en los diccionarios antes creados
{
    int modificadorcercania = 1;//este modificador es para la cercania 
    bool noapar = false;//un bool para saber si alguno de los operadores de aparicion esta ocurriendo
    bool cercanos=false;
    double scoretemp = 0;
    foreach (string word in forsearch)
    {
        if (opicer.ContainsKey(word) && dic.Value.ContainsKey(word)) //este if hace q si la palabra esta contenida en el diccionario q identifica cuales palabras estan modificadas por el operador cercania
        {
            cercanos = soncercanos(cercan[dic.Key], word, opicer[word]);//llama a este metodo q me dicen si las palabras se encuentran en algun texto juntas
        
        }
        
        
        
        
        if (opapar.ContainsKey(word) && opapar[word] == "!" && dic.Value.ContainsKey(word)) { noapar = true; }// en cuakquiera de los dos casos el bool es true
        if (opapar.ContainsKey(word) && opapar[word] == "^" && dic.Value.ContainsKey(word) == false) { noapar = true; }

            
            double modificadorimpor = 1;//este es el modificador del operador * 

            if (cercanos) { modificadorcercania = 3; }//en caso de ser cierto q se encuentren juntas  el score de ese documento aumenta X3
            
            if (invertedfrec.ContainsKey(word) && dic.Value.ContainsKey(word)) //si la palabra esta en el documento actual y en el idf entonces se le agrega un valor a ese documento en referencia a esa palabra
            {

                if (opimpor.ContainsKey(word))//si la palabra evaluada aparece modificada por el operador su scoretemp es aumentado
                {
                    modificadorimpor = opimpor[word] * modificadorimpor * 1.2; //por esta medida
                }



                scoretemp = scoretemp + (invertedfrec[word] * dic.Value[word] * modificadorimpor);//y asi con cada palabra de la busqueda}

            }

        

    }
    if (noapar == false) { score.Add(dic.Key, (scoretemp * modificadorcercania)); }//se agrega el resultado final del documento con su score
    //y si el bool es true no se agrega el documento al resultado



}




foreach(var dic in score)
{
    if (dic.Value == 0) {score.Remove(dic.Key); }

  }//con esto elimino los documentos q no coinciden con la busqueda del resultado, es decir los q tienen score=0
 
 cantidadderesults=score.Count;
//sugerncia
 if (score.Count < 10)
 {
    sugestion = string.Join(" ", sugerencia(forsearch, invertedfrec));
 }
 else { sugestion = string.Join(" ", forsearch); }


numeroderesultados=score.Count;
if(score.Count>5){numeroderesultados=5;}//la cantidad de resultados q voy a mostrar en pantalla es 5

if(ensenar==true)
    {
    numeroderesultados=score.Count;
    if(score.Count>10){numeroderesultados=10;}
    }



 SearchItem[] items=new SearchItem[numeroderesultados];// el array de search result q voy a rellenar ahora
 
 
  for (int i = 0; i <numeroderesultados ; i++)// por cada resultado saco el mayor valor del diccionario q vendria siendo el score
{
    var maxValueKey = score.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;//e=uso la funcion agregate donde uso una ternaria para determinar el mayor valor
    string snipet = "";
    StreamReader temptext = new StreamReader(maxValueKey);//cargo el documento en cuestion para obtener un snipett
    string[] tempwords = separator(temptext.ReadToEnd().ToLower());
    double maxtf = 0;
    string maxstf = "";
   
    foreach (string word in forsearch) {if(opapar.ContainsKey(word)==false || (opapar.ContainsKey(word) && opapar[word]=="^")) 
    {   if(termfrec[maxValueKey].ContainsKey(word)){ maxtf = Math.Max(termfrec[maxValueKey][word], maxtf);} }}// primero calculo de las palabras de la busqueda q aparecen en el doc la q tiene mayor tf
       

    foreach (string word in forsearch) 
    { if ((opapar.ContainsKey(word)==false || opapar.ContainsKey(word) && opapar[word]=="^") && termfrec[maxValueKey].ContainsKey(word) && termfrec[maxValueKey][word] == maxtf) 
    { maxstf = word; } }
    string[] tempsnipet = new string[50];
    int count = 0;
    for (int j = 0; j < tempwords.Length; j++)//a partir de esa palabra recojo las 50 siguientes
    {
        
        if (tempwords[j] == maxstf)
        {    
            tempwords[j] = "<mark style=\"background:white;padding:0;font-weight:bolder!important;font-size:20px\">"+ tempwords[j] +"</mark>";
            
            try
            {
                for (int h = j-7; h < j + 6; h++) 
                { 
                
                             
              
                tempsnipet[count] = tempwords[h]; count++; 
                }
            }
            catch{} 
            
            try{tempsnipet[count]=".........";count++;}
            
            catch{}
            
            snipet = string.Join(" ", tempsnipet);
            
        }

       

    }
items[i]= new SearchItem(title(maxValueKey),snipet,score[maxValueKey]);//agrego al valor del array de searchitem el titulo, el snipet y el score
score.Remove(maxValueKey);// y remuevo ese par del diccionario para repetir la operacion
}

  
score.Clear();
ensenar=false;
    return new SearchResult(items, sugestion);
}



}
