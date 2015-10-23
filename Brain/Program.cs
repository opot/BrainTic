using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {

	public enum CellState {
		Cross, Zero, Empty, Block
	}

	struct SolveResult {
		int row;
		float win_chance;

		public SolveResult(float win_chance) {
			this.row = 0;
			this.win_chance = win_chance;
		}

	}

	public sealed class Program {

		public Program(string fold, CellState player) {

			Field field = new Field(fold);

		}

		private SolveResult makeTurn(CellState player, int deep, Field field) {

			CellState field_state = field.check();
			if(field_state != CellState.Empty) {
				return new SolveResult(field_state == player?1:0);
			} else {
				int i = 0;
				SolveResult result = new SolveResult();

				return result;
			}

		}

		static void Main(string[] args) {
			new Program(args[0], args[1][0] == 'x'?CellState.Cross: CellState.Zero);
			Console.ReadKey();
		}
	}

}
