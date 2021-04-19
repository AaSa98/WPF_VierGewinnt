using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace _4Gewinnt
{
    /// <summary>
    /// Interaktionslogik für VierGewinnt.xaml
    /// </summary>
    
    

    public partial class VierGewinnt : Window
    {

        //Zweidimensionales Array mit allen Münzfeldern
        private int[,] slots;

        //Gibt an, ob das gesamte Spielfeld gerade für Eingaben (Klicks) gesperrt ist
        volatile private bool fieldLocked;

        // Welcher Spieler ist gerade dran?



        private int player = 1;

        public VierGewinnt()
        {
            InitializeComponent();

            // Spielfelder initialisieren
            slots = new int[,] {     { 0, 0, 0, 0, 0, 0, 0},
                                     { 0, 0, 0, 0, 0, 0, 0},
                                     { 0, 0, 0, 0, 0, 0, 0}, 
                                     { 0, 0, 0, 0, 0, 0, 0}, 
                                     { 0, 0, 0, 0, 0, 0, 0},
                                     { 0, 0, 0, 0, 0, 0, 0}
            };

            // [1] Münzfelder erstellen (Es existiert bereits eine Methode dafür)
            createFields();

            // [2] Spiel zurücksetzen (Es existiert bereits eine Methode dafür)
            resetGame();

        }

        /**
         * Erstellt alle Felder im Grid (müssen Studis selbst machen aber mit Hilfestellung)
         */
        public void createFields()
        {
            // Hinweis: Die Anzahl der Reihen und Spalten im Grid können mittels Field.RowDefinitions.Count bzw. FieldColumnDefinitions.Count abgeholt werden

            for (int row = 0; row < Field.RowDefinitions.Count; row++)
            {
                for (int column = 0; column < Field.ColumnDefinitions.Count; column++)
                {
                    // [3] Neues Münzfeld erstellen (Klasse Ellipse verwenden)
                    Ellipse current = new Ellipse();

                    // [4] Hintergrundfarbe des Münzfeldes auf neutrale Farbe (z.B. Weiß) setzen (Achtung. Bei Ellipse ist die Eigenschaft der Hintergrundfarbe "Fill")
                    current.Fill = System.Windows.Media.Brushes.White;

                    // [5] Münzfeld in der entsprechenden Zeile und Spalte platzieren sowie Rahmenstärke vorgeben (Vorgegeben)
                    current.SetValue(Grid.RowProperty, row);
                    current.SetValue(Grid.ColumnProperty, column);
                    current.Margin = new Thickness(5);

                    // [6] Klick Methode hinzufügen
	                current.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(SlotClicked));

                    // [7] Münzfelder dem Spielfeld (Grid) hinzufügen
                    Field.Children.Add(current);

                }
            }
        }


        /**
         * Setzt alle Münzfelder auf Weiß und alle Felder im Array auf 0 zurück (Müssen Studis selbst machen)
         */
        public void resetGame()
        {

            // Hinweis: Die Anzahl der Reihen und Spalten im Grid können mittels Field.RowDefinitions.Count bzw. FieldColumnDefinitions.Count abgeholt werden
            for (int row = 0; row < Field.RowDefinitions.Count; row++)
            {
                for (int column = 0; column < Field.ColumnDefinitions.Count; column++)
                {
                    // [3] Münzfeld mit aktuellem Zeilen und Spalten-Index abholen (Es existiert eine Methode dafür)
                    Ellipse current = getSlot(row, column);

                    // [4] Hintergrundfarbe des Münzfeldes auf neutrale Farbe (z.B. Weiß) setzen (Achtung. Bei Ellipse ist die Eigenschaft der Hintergrundfarbe "Fill")

                    current.Fill = System.Windows.Media.Brushes.White;

                    // [5] Im Array das entsprechende Münzfeld mit Zeilen und Spalten-Index zurücksetzen (0)
                    slots[row,column]=0;
                }
            }

            // [6] Aktuellen Spieler festlegen (1 oder 2)
            player = 1;

            /// Random Player festlegen
            ///Random rnd = new Random();
            ///player = rnd.Next(1, 3);


            // [7] Aktuellen Spieler auf Oberfläche aktualisieren (Es existiert eine Methode dafür)
            updatePlayerOnGUI();
            
            // [8] Gesamtes Spielfeld entsperren (Variable fieldLocked)
            fieldLocked=false;
            // [9] Spielergebnis (Gewonnen/Verloren) verstecken (Es existiert eine Methode dafür)
            showOrHideGameResultPanel(false);
            
        }
        
        /*
        * Holt einen Slot für eine Reihe/Spalte Kombination ab (Vorgegeben)
        * */
        private Ellipse getSlot(int row, int column)
        {
            return Field.Children.Cast<Ellipse>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == column);
        }


        public void SlotClicked(object sender, RoutedEventArgs e)
        {

            if (!fieldLocked) // Überprüfen ob gesamtes Spielfeld aktuell nicht gesperrt
            {
                fieldLocked=true;
                
                // [2] Ermitteln des angeklickten Münzfelds (Typ Ellipse; Casten!) 
                Ellipse slot = (Ellipse) sender;

                // [3] Ermitteln der Spalte anhand des Münzfelds (Vorgegeben)
                int column = (int)slot.GetValue(Grid.ColumnProperty);

                // [4] Freien Slot in der Spalte suchen
                // Hinweis: Beachten Sie, dass der höchste Reihenindex die untereste Reihe des Spielfelds darstellt, daher müssen Sie die Schleife umgekehrt durchlaufen lassen,
                // d.h. beim höchsten Index beginnen. Die Anzahl der Reihen können Sie mittels des Arrays ermitteln (slots.GetLength(0))
                for (int row = slots.GetLength(0)-1; row>=0 ; row-- /*Alle Reihen durchlaufen */ )
                {


                    if (slots[row,column]==0)/* [5] Mit Hilfe des Arrays überprüfen, ob das Münzfeld frei ist. (Dummy-Wert 'false' muss gelöscht werden) */
                    {

                        // [6] Wenn freier Slot --> anhand des gerade aktiven Spielers den Stein setzen, d.h. im Array den Wert 0 auf den Wert des aktuellen Spielers setzen (1 oder 2)
                        slots[row,column]=player;
                        // [7] Leeres Münzfeld an der Oberfläche einfärben (Rot = Spieler 1; Gelb = Spieler 2)
                        if(player==1)
                            {
                                getSlot(row,column).Fill=System.Windows.Media.Brushes.Red;
                            }
                        if(player==2)
                            {
                                getSlot(row,column).Fill=System.Windows.Media.Brushes.Yellow;
                            }
                        if(player==3)
                            {
                                getSlot(row,column).Fill=System.Windows.Media.Brushes.Green;
                            }
                        // Die Ellipse des ermittelten Münzfeldes können Sie mit Hilfe der Methode getSlot(zeile, spalte) abholen
                        

                        // [8] Aktuelles Spielfeld in der Konsole ausgeben (Es existiert bereits eine Methode dafür)
                        printOutPlayField();
                        
                        // [9] Überprüfen, ob Viererreihe existiert (Es existiert bereits eine Methode dafür; Zeile und Spalte müssen übergeben werden)
                        if (checkWin(row,column)==true) /* Check Viererreihe (Dummy-Wert 'false' muss gelöscht werden)  */
                        {
                            // [10] Spielergebnisfeld einblenden (Es existiert bereits eine Methode dafür)
                                showOrHideGameResultPanel(true);

                            // [11] Gewinnnachricht für den aktuellen Spieler im Spielergebnisfeld des GUI setzen (Sehen Sie im XAML nach wie das entsprechende Feld heißt)
                                GameResultTextBlock.Text="Spieler "+player+" hat gewonnen";
                            
                            return; // Methode wird abgebrochen (Vorgegeben)
                        }

                        // [12] Spieler wechseln (Wenn Spieler 1 --> Spieler 2 und umgekehrt)
                        // if(player==1)
                        //    {
                        //        player=2;
                        //    }
                        // else
                        //   {
                        //        player=1;
                        //    }
                        if (player == 1)
                            player = 2;
                       else if (player == 2)
                            player = 3;
                       else if (player == 3)
                            player = 1;
                        

                        // [13] GUI aktualisieren (Es exisitert bereits eine Methode dafür)
                        updatePlayerOnGUI();

                        break; // Schleife wird abgebrochen (Vorgegeben)
                    }
                }

                // [14] Gesamtes Spielfeld entsperren
                fieldLocked=false;
                
            }

        }

        /**
         * Gibt den Stand des Spielfelds in der Konsole aus (Debug: Sind die Felder in Grid und Array synchron?)
         */
        private void printOutPlayField()
        {

            //Hinweis: Die Anzahl der Reihen und Spalten kann über das Array ermittelt werden (Reihen: rows.GetLength(0); Spalten: slots.GetLength(1))

            for (int row = 0; row < Field.RowDefinitions.Count; row++)
            {
                for (int column = 0; column < Field.ColumnDefinitions.Count; column++)
                {
                    // [3] Inhalt des Array-Feldes ausgeben - Console Ausgabe

                }
                
                
            }

        }

        
        /**
         * Überprüft, ob das Spiel gewonnen wurde
         * */
        private bool checkWin(int row, int column)
        {
            // [1] Zähle Felder links und rechts vom angeklickten Feld (-)
            int slotsRight = countSlots(row, column + 1, 0, +1, player);
            int slotsLeft = countSlots(row, column - 1, 0, -1, player);

            // [2] Zähle Felder unter und über dem angeklickten Feld (|)
            int slotsDown = countSlots(row +1, column, +1, 0, player);
            int slotsUp = countSlots(row -1, column, -1, 0, player);

            // [3] Zähle Felder in der von links oben nach rechts unten verlaufenden Diagonale vom angeklickten Feld (\)
            int slotsDiagRightDown = countSlots(row +1, column +1, +1, 1 , player);
            int slotsDiagLeftUp = countSlots(row -1, column -1, -1, -1 , player);

            // [4] Zähle Felder in der von links unten nach rechts oben verlaufenden Diagonale vom angeklickten Feld (/)
            int slotsDiagLeftDown = countSlots(row +1, column -1, +1, -1 , player);
            int slotsDiagRightUp = countSlots(row -1, column +1, -1, +1 , player);

            // [5] Ermittele, ob die jeweils addierten untersuchten Teilhälften das Gewinnkriterium erfüllen
            bool leftAndRight = slotsRight+slotsLeft>=3;
            bool upAndDown = slotsUp+slotsDown>=3;
            bool firstDiag = slotsDiagRightDown+slotsDiagLeftUp>=3;
            bool secondDiag = slotsDiagLeftDown+slotsDiagRightUp>=3;

            // [6] Wenn eines der Kriterien erfüllt ist: Spiel gewonnen
            return leftAndRight || upAndDown || firstDiag || secondDiag;
        }


        /**
         * Zählt die aufeinanderfolgenden Felder eines Spielers ausgehend von der übergebenen Zeile (row) und Spalte (column)
         * 
         * row = aktuelle Reihe
         * column = aktuelle Spalte
         *
         * searchDirRow = Suchrichtung in der Zeile (1 = rechts, -1 = links)
         * searchDirColumn = Suchrichtung in der Spalte (-1 = oben, +1 = unten)
         *
         */
        private int countSlots(int row, int column, int searchDirRow, int searchDirCol, int player)
        {
            if (row >= 0 && row < slots.GetLength(0) && column >= 0 && column < slots.GetLength(1)) // Überprüfe Arraygrenzen
            {
                if (slots[row, column] == player) // Nur wenn Spieler gleich dem aktuellen Spieler ist, darf gezählt werden
                {
                    return 1 + countSlots(row + searchDirRow, column + searchDirCol, searchDirRow, searchDirCol, player); //Rekursiver Aufruf mit dem nächsten Feld
                }
            }
            return 0;
        }

        /**
         * Versteckt oder zeigt das Spielergebnis
         */
        private void showOrHideGameResultPanel(bool visible) 
        {
            GameResultPanel.Visibility = visible?Visibility.Visible:Visibility.Hidden;
        }

        /*
         * Aufruf über GUI. Startet neues Spiel
         */
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            // Spiel zurücksetzen (Es exisitiert eine Methode dafür)
            resetGame();
            
        }

        /**
         * Aktualisiert den aktuellen Spieler
         * 
         */
        private void updatePlayerOnGUI()
        {
            Player.Content=player;
            if(player==1)
                {
                    Player.Foreground=Brushes.Red;
                    GameResultPanel.Background=Brushes.Red;
                }
            else if(player==2)
                {
                    Player.Foreground=Brushes.Yellow;
                    GameResultPanel.Background=Brushes.Yellow;
                }
            else if(player==3)
                {
                    Player.Foreground=Brushes.Green;
                    GameResultPanel.Background=Brushes.Green;
                }
        }

        /**
         * Beendet das Spiel
         */
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
