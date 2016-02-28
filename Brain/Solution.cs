/*
 *************************************************************************
 * AI for "Connect Five" game.											 *
 *                                                                       *
 * This program should be used for Connect Five Competition.             *
 * Connect Five is the game like Connect Four; for more information see  *
 * http://www.math.spbu.ru/user/chernishev/connectfive/connectfive.html  *
 *                                                                       *
 * Author: Grigory Pervakov                                              *
 * Email: pervakov.grigory@gmail.com									 *
 * Year: 2015                                                            *
 * See the LICENSE file in the project root for more information.        *
 *************************************************************************
*/

namespace Brain {

	/// <summary>
	/// This class stores current solution
	/// </summary>
	public class Solution {

		private static double CHANCECOEFF = 100.0;

		Field field;
		Solution last;

		CellState player;
		int col;
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

		/// <summary>
		/// Constructor for a root
		/// </summary>
		/// <param name="field">State of the game field after start</param>
		/// <param name="player">Player's symbol</param>
		public Solution(Field field, CellState player) {
			this.field = field;
			this.player = player;
			last = null;
		}

		/// <summary>
		/// Constructor for a new branch
		/// </summary>
		/// <param name="last">"Parent" of this branch</param>
		/// <param name="row">The column AI tries to move</param>
		public Solution(Solution last, int row) {
			this.col = row;
			this.player = invertCell(last.player);
			if (!last.field.checkRow(row)) {
				Finalized = true;
				canMove = false;
				return;
			}

			field = new Field(last.field, row, player);
			this.last = last;
			CellState result = field.CheckField(row, player);
			if (result != CellState.Empty) {
				Finalized = true;
				mathWait = (player == result) ? 1 : -1;
				last.toStart(mathWait / CHANCECOEFF);//notify root branches
				if (result == player)
					last.canMove = false;
			}
		}

		/// <summary>
		/// Recursively sends results to the root
		/// </summary>
		/// <param name="mathWait">Leaf result</param>
		void toStart(double mathWait) {
			this.mathWait -= mathWait;
			if (last != null)
				last.toStart(-mathWait / CHANCECOEFF);
		}

		/// <summary>
		/// Inverts player ('X' -> 'O' or 'O' -> 'X')
		/// Has no effect on CellState.Block and CellState.Empty
		/// </summary>
		/// <param name="player">Symbol that should be inverted</param>
		/// <returns>Inverted player</returns>
		public static CellState invertCell(CellState player) {
			return (CellState)(-(int)player);
		}

		/// <summary>
		/// Returns the column that was used that time
		/// </summary>
		/// <returns>String with the column number</returns>
		public string getTurn() {
			return col.ToString();
		}

		/// <summary>
		/// Compares two branches
		/// </summary>
		/// <param name="other">branch to compare</param>
		/// <returns>"Greater" branch</returns>
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

			int delta = (col == Field.block1 || col == Field.block2) ? 1 : 0;
			if (this.field.cells[col][Field.SIZE - 2 - delta] == player)
				return this;

			return other;
		}
	}
}