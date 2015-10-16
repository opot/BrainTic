using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {

	public enum CellState {
		Cross, Zero, Empty, Blocked
	}

	public sealed class Program {

		public Program(string fold, CellState player) {

			Field field = new Field(fold);

		}

		static void Main(string[] args) {
			new Program(args[0], args[1][0] == 'x'?CellState.Blocked: CellState.Zero);
		}
	}

	struct Field {
		CellState[][] cells;

		public Field(String s) {
			cells = new CellState[10][];
			for (int i = 0; i < 10; i++) {
				cells[i] = new CellState[10];
				for (int j = 0; j < 10; j++)
					cells[i][j] = CellState.Empty;
			}

			

        }
	}

}
