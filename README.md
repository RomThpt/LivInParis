# LivInParis Graph Analysis Tool 🎯

A C# application for analyzing and visualizing graph data structures.

## 📋 Features

-   Graph loading from MTX files
-   Two representation modes:
    -   Adjacency List
    -   Adjacency Matrix
-   Graph analysis algorithms:
    -   Breadth-First Search (BFS) 🌳
    -   Depth-First Search (DFS) 🌲
    -   Cycle detection 🔄
    -   Connectivity checking ⛓️
-   Graph visualization with SkiaSharp 🎨

## 🚀 Getting Started

1. Make sure you have .NET 6.0 or later installed
2. Clone the repository
3. Place your MTX files in the `public` directory
4. Run the program using:

```bash
dotnet run
```

## 📊 Output

The program will:

-   Generate graph analysis results in the console
-   Create a PNG visualization of the graph

## 🛠️ Project Structure

-   `Program.cs`: Main entry point
-   `Models/`
    -   `Graphe.cs`: Graph implementation
    -   `GraphVisualizer.cs`: Visualization logic

## 💾 SQL Setup

### Prerequisites

-   MySQL installed via Homebrew
-   MySQL tools configured in zsh

### Database Configuration

1. Install MySQL if not already done:

```bash
brew install mysql
```

2. Start MySQL service:

```bash
brew services start mysql
```

3. Configure MySQL in your ~/.zshrc:

```bash
export PATH="/usr/local/mysql/bin:$PATH"
```

4. Reload your shell configuration:

```bash
source ~/.zshrc
```

## 📝 Example Usage

The program comes with a sample Karate Club dataset (`soc-karate.mtx`) demonstrating social network analysis.

## 📚 Citation

```bibtex
@inproceedings{nr-aaai15,
    title = {The Network Data Repository with Interactive Graph Analytics and Visualization},
    author={Ryan A. Rossi and Nesreen K. Ahmed},
    booktitle = {AAAI},
    url={http://networkrepository.com},
    year={2015}
}
```

Data source: [Network Repository](http://networkrepository.com)
