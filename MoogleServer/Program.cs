using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


var builder = WebApplication.CreateBuilder(args);
Stopwatch time=new Stopwatch();
time.Start();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
 
MoogleEngine.Moogle.main=MoogleEngine.Build.CreateDiccionary(MoogleEngine.Moogle.direccion);
MoogleEngine.Moogle.termfrec=MoogleEngine.Build.TF(MoogleEngine.Moogle.main);
MoogleEngine.Moogle.invertedfrec=MoogleEngine.Build.invertedFrecuency(MoogleEngine.Moogle.main,MoogleEngine.Moogle.direccion.Length);
MoogleEngine.Moogle.cercan=MoogleEngine.Build.cercania(MoogleEngine.Moogle.direccion);
time.Stop();
System.Console.WriteLine(time.Elapsed);
app.Run();

