﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeneticAlgorithms;
using GeneticAlgorithms.Example_Classes;
using ZedGraph;

namespace GeneticAlgorithmsGUI
{
    public partial class GUI : Form
    {
        private PointPairList avgFitnessList, avgLengthList, avgVariationList, maxFitnessList, minFitnessList;
        private LineItem avgFitnessCurve, avgLengthCurve, avgVariationCurve, maxFitnessCurve, minFitnessCurve;
        private GeneticSimulation<IntGene> GenSim = null;
        private MondlandungsSimulation MondSim = null;
        private IRecombinationProvider recombinationProvider = null;
        private ISelectionProvider selectionProvider = null;

        public GUI()
        {
            InitializeComponent();
            GraphPane p = zgc_Simulationsgraph.GraphPane;
            p.Title.Text = "Population";
            p.XAxis.Title.Text = "Runden";
            p.XAxis.Scale.Min = 1;
            p.XAxis.Scale.MaxAuto = true;
            p.YAxis.Title.Text = "Fitness";
            p.YAxis.Scale.Max = 1.0;
            p.YAxis.Scale.Min = 0.0;
            p.YAxisList.Add("Länge");
            p.YAxisList[1].IsVisible = false;
            p.YAxisList.Add("Variation");
            p.YAxisList[2].IsVisible = false;

            this.avgFitnessList = new PointPairList();
            this.avgLengthList = new PointPairList();
            this.avgVariationList = new PointPairList();
            this.maxFitnessList = new PointPairList();
            this.minFitnessList = new PointPairList();

            this.avgFitnessCurve = p.AddCurve("Ø Fitness", avgFitnessList, Color.Green, SymbolType.None);
            this.avgLengthCurve = p.AddCurve("Ø Länge", avgLengthList, Color.Purple, SymbolType.None);
            this.avgVariationCurve = p.AddCurve("Ø Varation", avgVariationList, Color.Yellow, SymbolType.None);
            this.minFitnessCurve = p.AddCurve("min. Fitness", minFitnessList, Color.Red, SymbolType.None);
            this.maxFitnessCurve = p.AddCurve("max. Fitness", maxFitnessList, Color.Blue, SymbolType.None);

            avgFitnessCurve.IsVisible = true;
            avgVariationCurve.IsVisible = false;
            avgLengthCurve.IsVisible = false;
            minFitnessCurve.IsVisible = false;
            maxFitnessCurve.IsVisible = false;

            for (int i = 0; i < 100; i++)
            {
                avgFitnessList.Add(i, i / 100.0f);
            }

            zgc_Simulationsgraph.AxisChange();

            cmb_Rekombinator.SelectedIndex = 0;
            recombinationProvider = new AsymmetricCrossoverRecombinator();
            cmb_Selektor.SelectedIndex = 1;
            selectionProvider = new PieCakeSelector();
        }

        private void GUI_Load(object sender, EventArgs e)
        {
            /*Graphics g = pe.Graphics;

            Pen bp = new Pen(Color.Black, 2);
            Pen sp = new Pen(Color.Red, 2);

            Point pt1 = new Point();
            Point pt2 = new Point();

            double ballX;
            double ballY;

            double ballRotX;
            double ballRotY;

            // Draw UFO
       //     g.DrawImage(
    
//            g.DrawLine(bp, pt1, pt2);*/

        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void überToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Mondlandungs-Simulation mit genetischen Algorithmen.\n\n © 2010 Daniel Baulig, Sven Sperner, Jonas Heil, Christian Kleemann");
        }

        private void txt_Int_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                Convert.ToInt32((sender as TextBox).Text);
            }
            catch (FormatException exception)
            {
                MessageBox.Show("Kein Ganzzahlwert!");
                (sender as TextBox).SelectAll();
                e.Cancel = true;
            }
        }

        private void txt_Float_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                Convert.ToDouble((sender as TextBox).Text);
            }
            catch (FormatException exception)
            {
                MessageBox.Show("Kein Gleitkommawert!");
                (sender as TextBox).SelectAll();
                e.Cancel = true;
            }
        }

        private void chk_AVGFitness_CheckedChanged(object sender, EventArgs e)
        {
            avgFitnessCurve.IsVisible = (sender as CheckBox).Checked;
            zgc_Simulationsgraph.GraphPane.YAxisList[0].IsVisible = chk_AVGFitness.Checked || chk_maxFitness.Checked || chk_minFitness.Checked;
            zgc_Simulationsgraph.Invalidate();
        }

        private void chk_Laenge_CheckedChanged(object sender, EventArgs e)
        {
            avgLengthCurve.IsVisible = (sender as CheckBox).Checked;
            zgc_Simulationsgraph.GraphPane.YAxisList[1].IsVisible = avgFitnessCurve.IsVisible = (sender as CheckBox).Checked;
            zgc_Simulationsgraph.Invalidate();
        }

        private void chk_Variation_CheckedChanged(object sender, EventArgs e)
        {
            avgVariationCurve.IsVisible = (sender as CheckBox).Checked;
            zgc_Simulationsgraph.GraphPane.YAxisList[2].IsVisible = avgFitnessCurve.IsVisible = (sender as CheckBox).Checked;
            zgc_Simulationsgraph.Invalidate();
        }

        private void chk_minFitness_CheckedChanged(object sender, EventArgs e)
        {
            minFitnessCurve.IsVisible = (sender as CheckBox).Checked;
            zgc_Simulationsgraph.GraphPane.YAxisList[0].IsVisible = chk_AVGFitness.Checked || chk_maxFitness.Checked || chk_minFitness.Checked;
            zgc_Simulationsgraph.Invalidate();
        }

        private void chk_maxFitness_CheckedChanged(object sender, EventArgs e)
        {
            maxFitnessCurve.IsVisible = (sender as CheckBox).Checked;
            zgc_Simulationsgraph.GraphPane.YAxisList[0].IsVisible = chk_AVGFitness.Checked || chk_maxFitness.Checked || chk_minFitness.Checked;
            zgc_Simulationsgraph.Invalidate();
        }

        private void btn_Zuruecksetzten_Click(object sender, EventArgs e)
        {
            GenSim = null;
            MondSim = null;
            cmb_Selektor.Enabled = true;
            cmb_Rekombinator.Enabled = true;
            txt_Chromosomlaenge.Enabled = true;
            txt_Gewicht.Enabled = true;
            txt_Hoehe.Enabled = true;
            txt_Treibstoff.Enabled = true;
            txt_Mutationsrate.Enabled = true;
            txt_Verlustrate.Enabled = true;
            txt_Duplikationsrate.Enabled = true;
        }

        private void btn_Simuliere_Click(object sender, EventArgs e)
        {
            if (GenSim == null)
            {
                txt_Chromosomlaenge.Enabled = false;
                cmb_Rekombinator.Enabled = false;
                cmb_Selektor.Enabled = false;
                txt_Gewicht.Enabled = false;
                txt_Hoehe.Enabled = false;
                txt_Treibstoff.Enabled = false;
                txt_Mutationsrate.Enabled = false;
                txt_Verlustrate.Enabled = false;
                txt_Duplikationsrate.Enabled = false;
                MondSim = new MondlandungsSimulation(Convert.ToInt32(txt_Hoehe.Text), Convert.ToInt32(txt_Treibstoff.Text), Convert.ToInt32(txt_Gewicht.Text));
                GenSim = new GeneticSimulation<IntGene>(100, Convert.ToInt32(txt_Chromosomlaenge.Text), MondSim, recombinationProvider, selectionProvider);
            }
            GenSim.RunSimulation(Convert.ToInt32(txt_Rundenazahl.Text));
        }

        private void cmb_Selektor_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0: // Alpha
                    selectionProvider = new AlphaSelector();
                    break;
                case 1: // Pie Cake
                    selectionProvider = new PieCakeSelector();
                    break;
                case 2: // Random
                    selectionProvider = new RandomSelector();
                    break;
                default:
                    selectionProvider = new PieCakeSelector();
                    break;
            }
        }

        private void cmb_Rekombinator_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0: // Crossover
                    recombinationProvider = new AsymmetricCrossoverRecombinator();
                    break;
                case 1: // Zip
                    recombinationProvider = new AsymmetricZipRecombinator();
                    break;
                default:
                    recombinationProvider = new AsymmetricCrossoverRecombinator();
                    break;
            }
        }

    }

}

