using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GolTime.Pdf
{
    public class ReservacionesDocument : IDocument
    {
        private readonly ReservacionesModel _data;

        public ReservacionesDocument(ReservacionesModel data)
        {
            _data = data;
        }

        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);

                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(Header);

                page.Content().Element(Content);

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        }


        private void Header(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text("GOLTIME")
                    .Bold()
                    .FontSize(22);

                column.Item().AlignCenter().Text("REPORTE DE RESERVACIONES")
                    .SemiBold()
                    .FontSize(15);

                column.Item().PaddingTop(15);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Fecha de generación: {_data.FechaGeneracion:dd/MM/yyyy HH:mm}");

                    row.RelativeItem().AlignRight().Text($"Período: {_data.FechaInicio:dd/MM/yyyy} - {_data.FechaFin:dd/MM/yyyy}");
                });

                column.Item().PaddingVertical(10).LineHorizontal(1);
            });
        }

        private void Content(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cliente").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Fecha").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Hora Inicio").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Hora Fin").Bold();
                });

                foreach (var item in _data.Reservaciones)
                {
                    table.Cell().Padding(5).Text(item.Client?.Nombre ?? "");
                    table.Cell().Padding(5).Text(item.Fecha.ToString("dd/MM/yyyy"));
                    table.Cell().Padding(5).Text(item.HoraInicio.ToString("HH:mm"));
                    table.Cell().Padding(5).Text(item.HoraFin.ToString("HH:mm"));
                }
            });
        }

        private void Footer(IContainer container)
        {
            container.PaddingTop(15).Column(column =>
            {
                column.Item().LineHorizontal(1);

                column.Item().PaddingTop(5).AlignRight().Text(
                    $"Total de reservaciones: {_data.Reservaciones.Count}")
                    .SemiBold();
            });
        }
    }
}
