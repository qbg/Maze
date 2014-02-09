# Maze

A maze generator that tries to generate "interesting" perfect 2D mazes.

## Overview

This program generates mazes from some combination of two techniques: turning a maze into a labryinth, and generating a maze from another maze. Mazes are turned into labryinths through the conventional technique of dividing the corridors in half. Mazes are generated from other mazes by creating a maze for each cell in the source maze and then connecting these mazes together in the same way that the cells in the source maze are connected.

## Command line syntax

The arguments to the Maze program form a small stack-based program that operates on the context maze. Any argument that is not a command listed below is pushed onto the stack.

### `seed`

Set the maze to be a small 2x2 seed maze.

### `lab`

Set the maze to be the labryinth form of the maze.

### `gen`

Takes in a scaling factor (an integer) from the stack.

Set the maze to be a result of generating a maze from the maze, using the given scaling factor.

### `trim`

Set the maze to be the solution of the maze.

### `render`

Takes in a filename and scaling factor (an integer).

Generates a .png image from the maze, saving it to the specified filename. The scaling factor is the size of the cells in pixels.

### `save`

Takes in a filename.

Saves the maze to the given filename using an ad-hoc binary format.

### `load`

Takes in a filename.

Set the maze to be the maze that was saved to the given filename.

## Examples

### Generate a small, basic maze

Generate a small maze and render it to maze.png with a cell size of 32.

`seed 11 gen maze.png 32 render`

### Generate a higher order maze

Generate a small maze, and then make a big maze from it. Render the result to maze.png with a cell size of 8.

`seed 7 gen 11 gen maze.png 8 render`

### Make a huge, extremely twisty, maze along with its solution

First, we generate the maze and save it (instead of rendering it) to twisty.maze.

`seed 7 gen lab 11 gen twisty.maze save`

Next, we render it to maze.png with a cell size of 8.

`twisty.maze load maze.png 8 render`

Finally, we render out the solution to solution.png with a cell size of 8.

`twisty.maze load trim solution.png 8 render`
