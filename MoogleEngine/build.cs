namespace MoogleEngine;

    

public static class Build
{
//metodos ejecutados en el build
public static Dictionary<string, Dictionary<string, int>> CreateDiccionary(string[] ruta)//este metodo devuelve un diccionario con el q voy a trabajar en la busqueda
{
    Dictionary<string, Dictionary<string, int>> main = new Dictionary<string, Dictionary<string, int>>();

    for (int i = 0; i < ruta.Length; i++)//en este ciclo voy a leer con stream reader cada una de las direcciones almacenadas en direccion
    {
        StreamReader temptext = new StreamReader(ruta[i]);       
        string[] tempwords = Moogle.separator(temptext.ReadToEnd().ToLower());
        Dictionary<string, int> tempsecondary = new Dictionary<string, int>();//este objeto es un sustituto del diccionario pequeño para poder trabajar con el
        foreach (string word in tempwords)//con este foreach pienso agregar al objeto cada una de las palabras de cada txt
        {
            if (tempsecondary.ContainsKey(word))//si la palabra ya esta en el diccionario solo se modifica el valor
            {
                tempsecondary[word]++;
            }
            else
            {
                tempsecondary.Add(word, 1); //sino se agrega la palabra    
            }

        }
        main.Add(ruta[i], tempsecondary);//al final se agrega al diccionario grande la direccion del archivo como llave y el objeto como valor

    }
    return main;
}



public static Dictionary<string,double> invertedFrecuency(Dictionary<string, Dictionary<string, int>> principal, double totalDocumentos)//con este metodo calculo el idf de todas las palabras de todos los documentos
{
    Dictionary<string, double> IDF = new Dictionary<string, double>();//este sera el diccionario resultante
    Dictionary<string, int> sectemp = new Dictionary<string, int>();//este sera un objeto para representar el diccionario anidado
    int temcount = 0;//esta variable la uso para tener un contador de la casi cantidad total de los documentos(digo casi xq no cuenta las palabras repetidas en un mismo documento)
    int i = 0;
    foreach (var diccionary in principal)//este foreach es solo para determinar el tamaño del array q voy a usar
    {   sectemp = diccionary.Value;
        temcount=temcount+sectemp.Count;
               
    }
    string[] words=new string[temcount];//con este array me sera mucho mas facil a la hora de calcular los idf de las palabras
   
    
    
    foreach(var diccionary in principal)//aqui lleno el array con las palabras de los documentos
    {
        sectemp = diccionary.Value;
        foreach(KeyValuePair<string,int> word in sectemp)
        {
           words[i]=word.Key;
            i++;//este contador es para ir asignandoles las palabras a espacios del array
        }


    }

   
    
    for(int j=0; j<words.Length; j++)//ya aqui empiezo a recorrer el array palabra por palabra, en un mismo documento no hay palabras repetidas pero en diferentes es posible
    {
        temcount = 0;
        foreach (var diccionary in principal)
        {
            sectemp=diccionary.Value;//uso el diccionario sectemp para igualarlo al diccionario anidado como hasta ahora
            if (sectemp.ContainsKey(words[j])) { temcount++; }// si la palabra aparece en el diccionario entonces añade valor al contador, al final el valor de este contador sera la cantidad de documentos donde aparece la palabra [i]

        }
        if (IDF.ContainsKey(words[j])) { }//si ya esta la palabra contenida en el diccionario idf no lo agrego, se debe tomar esta precaucion para q no intente añadir al diccionario llaves repetidas
        else { IDF.Add(words[j], Math.Log10(totalDocumentos / temcount)); }//si no esta contenida se añade con su idf


    }
    
    return IDF;
}



public static Dictionary<string,Dictionary<string,double>> TF(Dictionary<string, Dictionary<string, int>> principal) //en este metodo voy a crear otro diccionario de diccionario para guardar cada palabra con su tf en el documento en el q estan
{
    Dictionary<string,Dictionary<string,double>> result=new Dictionary<string,Dictionary<string,double>>();//creo el diccionario q voy a devolver
   
    foreach(var diccionary in principal)//recorro el diccionario base
    {
        Dictionary<string, double> dic = new Dictionary<string, double>();// en este diccionario pequeño guardo las palabras con su tf 
        double totalwords = 0;//contador de la cantidad de palabras del documento
        foreach (KeyValuePair<string,int> arc in diccionary.Value)//este es solo un ciclo para contar dicha cantidad total de palabras
        {
            totalwords = totalwords + arc.Value;
        }
        
        foreach(KeyValuePair<string,int> sec in diccionary.Value)//en este ciclo empiezo a recorrer el diccionario asociado al documento
        {
           
           dic.Add(sec.Key, (sec.Value/totalwords));//lo añado al diccionario de palabra por tf
            
        }


        result.Add(diccionary.Key, dic);// y al final lo agrego al diccionario q voy a dar como resultado


    }

    return result;
}



public static Dictionary<string,Dictionary<string,List<string>>> cercania(string[] paht)//este hasta ahora es el metodo mas enreversado todavia no se si funciona bien o no
{
    Dictionary<string, Dictionary<string, List<string>>> result = new Dictionary<string, Dictionary<string, List<string>>>();//para ayudarme con el operador cercania creo un diccionario de diccionarios de string con lista
   //la llave del diccionario grande es el titulo del documento,el valor es un diccionario donde su llave son las palabras de cada documento y su valor es una lista donde aparecen todas las palabras q se encuentran cercanas a ella
    for (int i = 0; i < paht.Length; i++)//un ciclo para pasar por cada documento
    {   Dictionary<string,List<string>> sec=new Dictionary<string,List<string>>();
        StreamReader temptex = new StreamReader(paht[i]);//leo el documento
        string[] tempwords =Moogle.separator(temptex.ReadToEnd().ToLower());
            
           for(int j = 0; j < tempwords.Length-15; j++)//voy a asociar a cada palabra con sus 15 palabras siguientes q es mi criterio de cercania
           {
            if (sec.ContainsKey(tempwords[j]))//en caso de q la palabra se repita y ya la haya agregado pues modifico y le agrego 15 palabras mas q son cercanas a ella 
            {   for (int h = j+1; h < j+16; h++) 
                {
                    sec[tempwords[j]].Add(tempwords[h]);               
                } 
            }
            else
            {
             List<string> list = new List<string>();
             for(int h = j+1;h < j+16; h++) //si no pues la agrego junto con sus 15 palabras siguientes
                { 
                  list.Add(tempwords[h]); 
                }
             sec.Add(tempwords[j], list);

            }
           }   
         result.Add(paht[i], sec); //y lo agrego todo al resultado    
    }




    return result;
}





















}