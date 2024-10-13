using Sudoku.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace Sudoku.Models
{
    //Сетка Судоки
    public class Grid
    {
        //Cвойство TotalRows используется
        //для получения общего количества строк в сетке


        public int TotalRows { get; private set; }

        //Свойство TotalColumns используется
        //для получения общего количества столбцов в сетке
        public int TotalColumns { get; private set; }

        //Свойство GridSize используется
        //для получения общего размера сетки
        public int GridSize { get; private set; }

        //Свойство SubGridSize используется
        //для получения размера подсетки
        public int SubGridSize { get; private set; }

        //свойство TotalCells используется
        //для получения общего количества ячеек в сетке
        public int TotalCells { get => TotalRows * TotalColumns; }

        //Свойство Cells используется
        //для получения и установки списка ячеек в сетке.
        public List<Cell> Cells { get; set; }

        //Свойство Solver используется
        //для получения решателя сетки
        public Solver Solver { get; }

        //Конструктор класса Grid
        public Grid(int totalRows, int totalColumns)
        {
            TotalRows = totalRows;
            TotalColumns = totalColumns;
            //Вычисление и установка значения общего размера сетки
            GridSize = Convert.ToInt16(Math.Sqrt(totalRows * totalColumns));
            //Вычисление и установка значения размера подсетки
            SubGridSize = Convert.ToInt16(Math.Sqrt(totalRows));
            //Создание нового списка ячеек с заданным размером
            Cells = new List<Cell>(TotalCells);
            //Создание нового объекта решателя сетки,
            //передавая текущую сетку как параметр конструктора
            Solver = new Solver(this);
            InitializeCells();
        }

        //метод IsGridFilled() возвращает true,
        //если все ячейки в сетке заполнены значениями отличными от -1,
        //иначе возвращает false.
        public bool IsGridFilled() => Cells.FirstOrDefault(cell => cell.Value == -1) == null;

        //метод IsGridEmpty() возвращает true,
        //если все ячейки в сетке пусты (содержат значение -1),
        //иначе возвращает false
        public bool IsGridEmpty() => Cells.FirstOrDefault(cell => cell.Value != -1) == null;

        //Метод Clear() устанавливает значение -1 для всех ячеек в сетке
        public void Clear() => Cells.ForEach(cell => SetCellValue(-1, cell.Index));

        //Метод GetCell() возвращает ячейку в сетке по заданному индексу
        public Cell GetCell(int cellIndex) => Cells[cellIndex];

        //Метод SetCellValue() устанавливает значение
        //ячейки по индексу cellIndex на значение value
        public void SetCellValue(int cellIndex, int value) => Cells[cellIndex].Value = value;

        //Метод InitializeCells() инициализирует ячейки путем создания
        //объектов Cell и добавления их в коллекцию Cells
        private void InitializeCells()
        {
            //Проходим по каждой строке
            for (var x = 0; x < TotalRows; x++)
            {
                //Проходим по каждому столбцу
                for (var y = 0; y < TotalColumns; y++)
                {
                    //Вычисляем значение группы, к которой принадлежит ячейка
                    var groupDivider = Convert.ToInt32(Math.Sqrt(TotalRows));

                    //Создаем объект Cell с заданными параметрами
                    //index - уникальный индекс ячейки
                    //value - значение ячейки
                    //groupNumber - номер группы, к которой принадлежит ячейка
                    //position - позиция ячейки (ряд, столбец)
                    Cells.Add(new Cell(
                        index: x * TotalRows + y,
                        value: -1,
                        groupNumber: (x / groupDivider) + groupDivider * (y / groupDivider) + 1,
                        position: new Position(row: x + 1, column: y + 1)
                    ));
                }
            }
        }
    }


    //Позиция строк и столбцов ячейки сетки
    public class Position
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public Position(int row, int column) { Row = row; Column = column; }
    }


    //Сведения об индексе, значении, номере группы и позиции ячейки
    public class Cell
    {
        public bool IsNote { get; set; } = false;
        public bool IsMutable { get; set; } = false;
        public int Index { get; private set; }
        public int Value { get; set; }
        public int GroupNumber { get; private set; }
        public Position Position { get; private set; }
        public Cell(int index, int value, int groupNumber, Position position)
        {
            Index = index;
            Value = value;
            GroupNumber = groupNumber;
            Position = position;
        }
    }
}