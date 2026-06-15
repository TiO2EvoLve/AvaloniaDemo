using Scriban;
using Xunit.Abstractions;

namespace xUnitTest.模板引擎;

public class Scriban(ITestOutputHelper TS)
{
    [Fact]
    public void Run()
    {
        string logicTemplateCode = @"
<ul>
{{- for product in products -}}
{{- if product.price > 100 }}
    <li><strong>{{ product.name }}</strong> - Price: {{ product.price }} (Premium)</li>
{{- else }}
    <li>{{ product.name }} - Price: {{ product.price }}</li>
{{- end }}
{{- end }}
</ul>";

        var template = Template.Parse(logicTemplateCode);
        var productsData = new
        {
            Products = new[]
            {
                new { Name = "高端鼠标", Price = 199m },
                new { Name = "普通键盘", Price = 89m }
            }
        };

        var result = template.Render(productsData);
        TS.WriteLine(result);
    }
}