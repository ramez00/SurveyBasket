namespace SurveyBasket.Helpers;

public static class EmailBodyBuilder
{
    public static string GenerateEmailBody(string template, Dictionary<string,string> templateValues)
    {
        var teplatePath = $"{Directory.GetCurrentDirectory()}/Templates/{template}.html";
        var streamReader = new StreamReader(teplatePath);
        var body = streamReader.ReadToEnd();
        streamReader.Close();
        streamReader.Dispose();

        foreach (var templateValue in templateValues)
            body = body.Replace(templateValue.Key, templateValue.Value);
        
        return body;
    }
}