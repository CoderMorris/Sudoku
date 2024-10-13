using Sudoku.Constants;
using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Helpers
{
    //Класс Generator используется для генерации сетки Судоки
    public class Generator
    {
        // Поле grid хранит ссылку на экземпляр класса Grid,
        // который представляет сетку Судоки
        readonly Grid grid;
        // Поле mode хранит режим генерации сетки Судоки
        private readonly string mode;
        // Поле solver хранит экземпляр класса Solver,
        // который используется для решения судоку
        private readonly Solver solver;
        // Поле random хранит экземпляр класса Random,
        // который используется для генерации случайных чисел
        private readonly Random random = new Random();

        private bool solved;
        // Конструктор класса Generator принимает ссылку
        // на экземпляр класса Grid и режим генерации сетки
        public Generator(Grid gridInstance, string gridMode)
        {
           // Если gridInstance равен null, то создается
           // новый экземпляр класса Grid с размерами 9*9
           grid = gridInstance ?? new Grid(9, 9);
           mode = gridMode;
           solver = new Solver(grid);
        }

        public void GetSolvedArr(out bool solved, out int[] gridarr)
        {
            solver.GetSolvedArr(out gridarr);
            solved = this.solved;
        }
        // Метод Generate генерирует сетку Судоки
        public bool Generate()
        {
            solved = solver.Solve();
            GenerateGrid();
            // Возвращаем true, чтобы указать, что генерация была успешной
            return true;
        }

        // Метод GenerateGrid генерирует значения ячеек сетки
        private void GenerateGrid()
        {
            // Переменная cellValueIndexes хранит список индексов ячеек,
            // которые будут содержать значения
            var cellValueIndexes = (mode, grid.GridSize) switch
            {
                // В зависимости от режима и размера сетки,
                // генерируется нужное количество индексов ячеек
                (Constants.Constants.Hard, 9) => GenerateRandomIndexes(random.Next(16, 24)),
                (Constants.Constants.Medium, 9) => GenerateRandomIndexes(random.Next(24, 31)),
                _ =>  GenerateRandomIndexes(random.Next(31, 39))
            };

            // Проходим по всем ячейкам сетки и устанавливаем значения -1 для ячеек,
            // чьи индексы не содержатся в cellValueIndexes
            foreach (var cell in grid.Cells)
            {
                if (!cellValueIndexes.Contains(cell.Index))
                {
                    cell.Value = -1;
                    cell.IsMutable = true;
                }
            }
        }

        //Метод GenerateRandomIndexes генерирует случайные индексы ячеек
        private List<int> GenerateRandomIndexes(int requiredNumbers)
        {
            // Генерируем список индексов, выбирая их случайным образом
            // из диапазона от 0 до общего количества ячеек в сетке
            return Enumerable.Range(0, requiredNumbers).Select(x => random.Next(0, grid.TotalCells)).ToList();
        }
    }


    // Класс Solver используется для решения сетки судоку 
    //с использованием процесса обратного отслеживания.
    public class Solver
    {
        readonly Grid grid;
        private readonly List<int> filledCells = new List<int>();
        private List<List<int>> blackListCells;
        private readonly Random random = new Random();
        private int[] gridarr = new int[81];
        
        public void GetSolvedArr(out int[] gridarr)
        {
            gridarr =  this.gridarr;
        }

        public int GetSolvedValue(int index)
        {
            return gridarr[index];
        }
        public Solver(Grid gridInstance)
        {
            grid = gridInstance ?? new Grid(9, 9);
            InitializeBlackList();
        }

        public bool Solve()
        {

            if (!ValidateGrid()) return false;

            IntializeFilledCells();

            ClearBlackList();

            int currentCellIndex = 0;


            while (currentCellIndex < grid.TotalCells)
            {

                if (filledCells.Contains(currentCellIndex))
                {
                    ++currentCellIndex;
                    continue;
                }

                ClearBlackList(cleaningStartIndex: currentCellIndex + 1);

                Cell currentCell = grid.GetCell(cellIndex: currentCellIndex);

                int foundNumber = GetValidNumberForTheCell(currentCellIndex);

                if (foundNumber == 0)
                    currentCellIndex = BackTrackTo(currentCellIndex);
                else
                {

                    grid.SetCellValue(currentCell.Index, foundNumber);
                    gridarr[currentCellIndex] = foundNumber;
                    ++currentCellIndex;
                }
            }

            return true;
        }


        public bool ValidateGrid(bool ignoreEmptyCells = false) =>
            grid.Cells.Where(cell => cell.Value != -1)
            .FirstOrDefault(cell => cell.Value != -1 && !IsValidValueForTheCell(cell.Value, cell)) == null;


        public bool IsValidValueForTheCell(int value, Cell cell)
        {
            var matchedCell = grid.Cells
                .Where(cellItem => cellItem.Index != cell.Index && (cellItem.GroupNumber == cell.GroupNumber
                || cellItem.Position.Row == cell.Position.Row || cellItem.Position.Column == cell.Position.Column))
                .FirstOrDefault(prop => prop.Value == value);

            return matchedCell == null;
        }
        


        private void IntializeFilledCells()
        {
            filledCells.Clear();
            filledCells.AddRange(grid.Cells.FindAll(cell => cell.Value != -1).Select(cell => cell.Index));
        }

        private void InitializeBlackList()
        {
            blackListCells = new List<List<int>>(grid.TotalCells);
            for (int index = 0; index < blackListCells.Capacity; index++)
                blackListCells.Add(new List<int>());
        }

       
        private int BackTrackTo(int index)
        {
            
            while (filledCells.Contains(--index)) ;

           
            Cell backTrackedCell = grid.GetCell(index);

            
            AddToBlacklist(backTrackedCell.Value, cellIndex: index);

            
            backTrackedCell.Value = -1;

           
            ClearBlackList(cleaningStartIndex: index + 1);

            return index;
        }

       
        private int GetValidNumberForTheCell(int cellIndex)
        {
            int foundNumber = 0;
            var possibleNumbers = Enumerable.Range(1, grid.GridSize).ToList();

            
            var validNumbers = possibleNumbers.Where(val => !blackListCells[cellIndex].Contains(val)).ToArray();

            if (validNumbers.Length > 0)
            {
                
                int choosenIndex = random.Next(validNumbers.Length);
                foundNumber = validNumbers[choosenIndex];
            }

            
            do
            {
                Cell currentCell = grid.GetCell(cellIndex);

                
                if (foundNumber != 0 && !grid.Solver.IsValidValueForTheCell(foundNumber, currentCell))
                    AddToBlacklist(foundNumber, cellIndex);
                else
                    break;

                
                foundNumber = GetValidNumberForTheCell(cellIndex: cellIndex);
            } while (foundNumber != 0);

            return foundNumber;
        }

        
        private void AddToBlacklist(int value, int cellIndex) => blackListCells[cellIndex].Add(value);

       
        private void ClearBlackList(int cleaningStartIndex = 0)
        {
            for (int index = cleaningStartIndex; index < blackListCells.Count; index++)
                blackListCells[index].Clear();
        }
    }
}
