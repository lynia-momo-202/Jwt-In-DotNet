# Étapes de développement et installation de la solution

## 1. Installation et initialisation du projet

### Prérequis
- .NET SDK installé
- Un éditeur de code (ex: Visual Studio Code)

### Création du projet
```bash
dotnet new webapi -n MonAPI
cd MonAPI
```

### Installation des dépendances JWT
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Microsoft.IdentityModel.Tokens
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package System.IdentityModel.Tokens.Jwt
```

## 2. Configuration de l'authentification JWT

### Generation des cles openssl
- Installation d'openssl
    - on windows
        [Installer OpenSSL](https://www.tbs-certificats.fr/FAQ/fr/openssl-windows.html)

    - on linus 
        ```bash
        sudo apt update && sudo apt install openssl -y
        ```

- Creer le dossier `Keys`
```bash
cd Keys
```
- Generer une clé publique et une cle prive
```bash
# 1. Générer une clé privée RSA
openssl genpkey -algorithm RSA -out private_key.pem

# 2. Extraire la clé publique à partir de la clé privée
openssl rsa -pubout -in private_key.pem -out public_key.pem
```

- Modifier le fichier `Program.cs` pour ajouter la configuration JWT.
- Exemple d'ajout dans `Program.cs` :
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Configuration des paramètres de validation
        };
    });
```

## 3. Création des modèles et contrôleurs

- Générer un modèle utilisateur :
```bash
dotnet add class User.cs
```
- Créer un contrôleur d'authentification :
```bash
dotnet add class AuthController.cs
```

## 4. Lancement et test de l'API

### Démarrer le projet
```bash
dotnet run
```

### Tester les endpoints avec curl
```bash
curl -X POST https://localhost:5001/api/auth/login -d '{"username":"user","password":"pass"}' -H "Content-Type: application/json"
```

## 5. Documentation et maintenance

- Documenter les endpoints dans le fichier README.
- Mettre à jour la documentation à chaque ajout de fonctionnalité.

> Ajoutez chaque étape et commande utilisée au fur et à mesure pour garder une trace claire du processus de développement et d'installation.