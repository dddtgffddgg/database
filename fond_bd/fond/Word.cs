using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;


namespace fond
{
    public class WordExporter
    {
        public void ExportToWord(DataGridView dgv, string additionalText, params int[] excludedColumns)
        {
            if (dgv == null || dgv.Rows.Count <= 0)
            {
                MessageBox.Show("Данные для экспорта не обнаружены.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;
            var wordDoc = wordApp.Documents.Add();

            try
            {
                InsertDataWord(wordDoc, additionalText, dgv, excludedColumns);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Marshal.ReleaseComObject(wordDoc);
                Marshal.ReleaseComObject(wordApp);
            }
        }

        private void AddTextToDocument(Document doc, string text)
        {
            // Определяем начало документа
            Range startRange = doc.Range();
            startRange.Collapse(WdCollapseDirection.wdCollapseStart);

            // Вставляем дополнительный текст
            startRange.Text = text + "";
            startRange.InsertParagraphAfter();
            startRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // Вставляем текст "Отчет"
            Range reportRange = doc.Range(startRange.End, startRange.End);
            reportRange.Text = "Отчет.";
            reportRange.Font.Bold = 1;
            reportRange.Font.Size += 2;
            reportRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            reportRange.InsertParagraphAfter();

            // Вставляем текст "Ниже представлен список данных волонтерской помощи:"
            Range tableTextRange = doc.Range(reportRange.End, reportRange.End);
            tableTextRange.Text = "Данный отчет представляет собой анализ волонтерской деятельности, оказанной получателям в рамках нашей программы помощи. В таблице ниже вы найдете информацию о получателях, волонтерах, а также виде помощи, предоставленной каждому получателю. Эти данные отражают усилия и вклад каждого волонтера в облегчение бремени тех, кто нуждается в нашей поддержке.";
            tableTextRange.InsertParagraphAfter();
        }

        private void AddCombinedNameColumns(DataGridView dgv)
        {
            const string needyColumnName = "ФИО Получателя";
            const string volunteerColumnName = "ФИО Волонтера";

            // Проверяем, существуют ли уже эти столбцы
            if (!dgv.Columns.Contains(needyColumnName))
            {
                dgv.Columns.Add(needyColumnName, needyColumnName);
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var lastName = row.Cells[2].Value?.ToString() ?? string.Empty;
                        var firstName = row.Cells[3].Value?.ToString() ?? string.Empty;
                        var middleName = row.Cells[4].Value?.ToString() ?? string.Empty;
                        row.Cells[dgv.Columns[needyColumnName].Index].Value = $"{lastName} {firstName} {middleName}".Trim();
                    }
                }
            }

            if (!dgv.Columns.Contains(volunteerColumnName))
            {
                dgv.Columns.Add(volunteerColumnName, volunteerColumnName);
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var lastName = row.Cells[6].Value?.ToString() ?? string.Empty;
                        var firstName = row.Cells[7].Value?.ToString() ?? string.Empty;
                        var middleName = row.Cells[8].Value?.ToString() ?? string.Empty;
                        row.Cells[dgv.Columns[volunteerColumnName].Index].Value = $"{lastName} {firstName} {middleName}".Trim();
                    }
                }
            }
        }


        private void InsertDataWord(Document doc, string textBeforeTable, DataGridView dgv, params int[] excludedColumns)
        {
            AddTextToDocument(doc, textBeforeTable);

            // Получаем диапазон для новой таблицы
            Range tableRange = doc.Range();
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // Добавляем таблицу
            var table = doc.Tables.Add(tableRange, dgv.Rows.Count + 1, 4); // Указываем 4 столбца

            // Задаем заголовки столбцов
            table.Rows[1].Cells[1].Range.Text = "Номер";
            table.Rows[1].Cells[2].Range.Text = "ФИО Получателя";
            table.Rows[1].Cells[3].Range.Text = "ФИО Волонтера";
            table.Rows[1].Cells[4].Range.Text = "Название помощи";

            // Заполняем таблицу данными из DataGridView
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                table.Rows[i + 2].Cells[1].Range.Text = (i + 1).ToString(); // Номер строки
                table.Rows[i + 2].Cells[2].Range.Text = $"{dgv.Rows[i].Cells[2].Value} {dgv.Rows[i].Cells[3].Value} {dgv.Rows[i].Cells[4].Value}"; // ФИО Получателя
                table.Rows[i + 2].Cells[3].Range.Text = $"{dgv.Rows[i].Cells[6].Value} {dgv.Rows[i].Cells[7].Value} {dgv.Rows[i].Cells[8].Value}"; // ФИО Волонтера
                table.Rows[i + 2].Cells[4].Range.Text = dgv.Rows[i].Cells[10].Value?.ToString(); // Название помощи
            }

            // Оформление таблицы
            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            table.Range.ParagraphFormat.SpaceAfter = 6;
            table.Borders.Enable = 1;

            AddSignatureAndDate(doc);
        }

        private void AddSignatureAndDate(Document doc)
        {
            // Вставляем текст "Подпись" слева и текущую дату справа в один абзац
            Range signatureRange = doc.Range();
            signatureRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // Создаем параграф с табуляцией
            object rangeObj = signatureRange;
            Paragraph para = doc.Content.Paragraphs.Add(ref rangeObj);
            para.Format.TabStops.Add(400, WdTabAlignment.wdAlignTabRight, WdTabLeader.wdTabLeaderSpaces);

            // Вставляем текст "Подпись" и текущую дату с табуляцией
            signatureRange.Text = "\n\nПодпись:____________________\t\t\t" + DateTime.Now.ToShortDateString();
            signatureRange.Font.Bold = 1;
            signatureRange.Font.Size += 2;
            signatureRange.InsertParagraphAfter();
        }
    }
}