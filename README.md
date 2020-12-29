# Credere

## Descrição
Uma sonda exploradora da NASA pousou em marte. O pouso se deu em uma área retangular, na qual a sonda pode navegar usando uma interface web. A posição da sonda é representada pelo seu eixo x e y, e a direção que ele está apontado pela letra inicial, sendo as direções válidas:

* E - Esquerda
* D - Direita
* C - Cima
* B - Baixo

A sonda aceita três comandos:

* GE - girar 90 graus à esquerda
* GD - girar 90 graus à direta
* M - movimentar. Para cada comando M a sonda se move uma posição na direção à qual sua face está apontada.

A sonda inicia no quadrante (x = 0, y = 0), o que se traduz como a casa mais inferior da esquerda, também inicia com a face para a direita. Se pudéssemos visualizar a posição inicial, seria:

(0,4) | (1,4) | (2,4) | (3,4) | (4,4)
--- | --- | --- | --- | ---
(0,3) | (1,3) | (2,3) | (3,3) | (4,3)
(0,2) | (1,2) | (2,2) | (3,2) | (4,2)
(0,1) | (1,1) | (2,1) | (3,1) | (4,1)
  *> | (1,0) | (2,0) | (3,0) | (4,0)

```
* Indica a direção inicial da nossa sonda
```

A intenção é controlar a sonda enviando a direção e quantidade de movimentos que ela deve executar. A resposta deve ser sua coordenada final caso o ponto se encontre dentro do quadrante, caso o ponto não possa ser alcançado a resposta deve ser um erro indicando que a posição é inválida. Para a execução do teste as dimensões de 5x5 será utilizado.

## Endpoints
Existem três endpoints, um que envia a sonda para a posição inicial (0,0), outro recebe o(s) movimento(s) (aceita uma série de movimentos que a sonda pode executar) e responde com as coordenadas finais, caso o movimento seja válido ou erro caso o movimento seja inválido, e o terceiro responde apenas com as coordenadas atuais X e Y da sonda.

Consideramos que um movimento para cima é o mesmo que dizer (x + 1, y) e um movimento para a direita é o mesmo que (x, y + 1).


### Endpoint: SetPosition
Realiza a movimentação da sonda através das coordenadas recebidas e retorna a descrição dos movimentos realizados.

Exemplo:

```
{
  movimentos: ['GE', 'M', 'M', 'M', 'GD', 'M', 'M']
}
```

Resposta:

```
{
    "position": [
        {
            "x": "3"
        },
        {
            "y": "2"
        },
        {
            "face": "D"
        },
        {
            "movementsPerformed": [
                "girou para a esquerda",
                "se moveu 3 vez(es) no eixo X",
                "girou para a direita",
                "se moveu 2 vez(es) no eixo Y"
            ]
        }
    ]
}
```

Postman exemplo:

```
.../Sonda.asmx/SetPosition?coordinates=GE,M,M,M,GD,M,M
```

A visualização da posição após esses movimentos seria a seguinte:

(0,4) | (1,4) | (2,4) | (3,4) | (4,4)
--- | --- | --- | --- | ---
(0,3) | (1,3) |  > | (3,3) | (4,3)
(0,2) | (1,2) | (2,2) | (3,2) | (4,2)
(0,1) | (1,1) | (2,1) | (3,1) | (4,1)
(0,0) | (1,0) | (2,0) | (3,0) | (4,0)

Exemplos de movimento inválido seriam os seguintes:

```
{
  movimentos: ['GD', 'M', 'M']
}
```

ou

```
{
  movimentos: ['M', 'M', 'M', 'M', 'M', 'M']
}
```

A resposta será:

```
{
    "error": "Um movimento inválido foi detectado, infelizmente a sonda ainda não possui a habilidade de voar."
}
```

O erro aconteceu porque no primeiro caso a sonda estava no ponto inicial e foi virada para a direção fora do limite do quadrante. No segundo caso a sonda ultrapassou o limite que era de cinco casas, tentando se mover para a posição y = 0, x= 6, passando do quadrante limite.

### Endpoint: GetPosition
Obtém a posição atual da sonda.

Postman exemplo:

```
.../Sonda.asmx/GetPosition
```

A resposta será:

```
{
    "position": [
        {
            "x": "3"
        },
        {
            "y": "2"
        },
        {
            "face": "D"
        }
    ]
}
```

### Endpoint: ResetPosition
Reinicia/move a sonda para a posição inicial (x = 0, y = 0, face = D).

Postman exemplo:

```
.../Sonda.asmx/ResetPosition
```

A resposta será:

```
{
    "message": "Posição da sonda reiniciada com sucesso!"
}
```
