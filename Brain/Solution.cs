using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {
	public class Solution {
		Field field;
		Solution last;

		CellState player;

		int row;

		double mathWait = 0;

		private bool canMove = true;
		private bool Finalized = false;

		public bool isMove {
			get {
				return canMove;
			}
		}
		public bool isFinalized {
			get {
				return Finalized;
			}
		}

		public Solution(Field field, CellState player) {
			this.field = field;
			this.player = player;
			last = null;
		}

		public Solution(Solution last, int row) {
			this.row = row;
			this.player = invertCell(last.player);
			if (!last.field.checkRow(row)) {
				Finalized = true;
				canMove = false;
				return;
			}

			field = new Field(last.field, row, player);
			this.last = last;
			CellState res = field.CheckField(row, player);
			if (res != CellState.Empty) {
				Finalized = true;
				mathWait = (player == res) ? 1 : -1;
				last.toStart(mathWait);
				if (res == player)
					last.canMove = false;
			}
		}

		void toStart(double mathWait) {

			this.mathWait -= mathWait;

			if(last != null)
				last.toStart(-mathWait/10.0);
		}

		public static CellState invertCell(CellState player) {
			return (CellState)(-(int)player);
		}

		public string getTurn() {
			return row.ToString();
		}

		public Solution isGreater(Solution other) {

			if (other == null)
				return this;

			if (!other.canMove)
				return this;
			if (!this.canMove)
				return other;

			if (mathWait > other.mathWait)
				return this;
			if (mathWait < other.mathWait)
				return other;

			int delta = (row == 4 || row == 5) ? 1 : 0;
            if (this.field.cells[row][8 - delta] == player)
				return this;

			return other;

		}

	}
}
