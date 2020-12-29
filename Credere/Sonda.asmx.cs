using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace Credere
{
    /// <summary>
    /// Uma sonda exploradora da NASA pousou em marte.
    /// O pouso se deu em uma área retangular de 5 x 5, na qual a sonda pode navegar usando os métodos aqui disponibilizados.
    /// A posição da sonda é representada pelo seu eixo x e y, e a direção que ele está apontado pela letra inicial, sendo as direções válidas:
    ///  * E - Esquerda
    ///  * D - Direita
    ///  * C - Cima
    ///  * B - Baixo
    /// A sonda aceita três comandos:
    ///  * GE - Girar 90 graus à esquerda
    ///  * GD - Girar 90 graus à direta
    ///  * M  - Movimentar. Para cada comando M a sonda se move uma posição na direção à qual sua face está apontada.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que esse serviço da web seja chamado a partir do script, usando ASP.NET AJAX, remova os comentários da linha a seguir. 
    // [System.Web.Script.Services.ScriptService]
    public class Sonda : System.Web.Services.WebService
    {
        #region Public WebMethod

        [WebMethod(Description = "Reinicia/move a sonda para a posição inicial (x = 0, y = 0, face = Direita).",
            EnableSession = true),
            ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ResetPosition()
        {
            HttpContext.Current.Response.ContentType = "application/json";

            try
            {
                ResetSessionVariables();

                Object returnResetPosition = new
                {
                    message = "Posição da sonda reiniciada com sucesso!"
                };

                HttpContext.Current.Response.Write(JavaScriptSerializer(returnResetPosition));
            }
            catch (Exception ex)
            {
                Object returnException = new
                {
                    error = ex.Message
                };

                HttpContext.Current.Response.Write(JavaScriptSerializer(returnException));
            }
        }

        [WebMethod(Description = "Realiza a movimentação da sonda através das coordenadas recebidas e retorna a descrição dos movimentos realizados.",
            EnableSession = true)]
        public void SetPosition()
        {
            HttpContext.Current.Response.ContentType = "application/json";

            try
            {
                if (HttpContext.Current.Request.QueryString["coordinates"] == null)
                {
                    throw new Exception("Nenhum movimento foi informado, a sonda precisa das coordenadas para poder mover-se...");
                }

                string[] coordinates = HttpContext.Current.Request.QueryString["coordinates"].Split(',');

                string movement = string.Empty;
                string previousMove = string.Empty;
                string movementPerformed = string.Empty;

                string messageInvalidMove = "Um movimento inválido foi detectado, infelizmente a sonda ainda não possui a habilidade de voar.";

                int x1AxisCounter = 0;
                int x2AxisCounter = 0;
                int y1AxisCounter = 0;
                int y2AxisCounter = 0;

                List<Object> movementsPerformed = new List<Object>();

                for (int i = 0; i < coordinates.Length; i++)
                {
                    movement = coordinates[i];

                    if (i > 0 && previousMove != movement)
                    {
                        x1AxisCounter = 0;
                        x2AxisCounter = 0;
                        y1AxisCounter = 0;
                        y2AxisCounter = 0;

                        movementsPerformed.Add(movementPerformed);
                    }

                    if (CheckValidMovement(movement))
                    {
                        if (movement.Equals("GE"))
                        {
                            SetFace("GE");

                            movementPerformed = "girou para a esquerda";

                            previousMove = movement;
                        }
                        else if (movement.Equals("GD"))
                        {
                            SetFace("GD");

                            movementPerformed = "girou para a direita";

                            previousMove = movement;
                        }
                        else if (movement.Equals("M"))
                        {
                            if (CanMove())
                            {
                                switch (GetFace())
                                {
                                    case "D":
                                        y1AxisCounter++;

                                        SetY(1);
                                        SetAllowedMovementsRight(GetAllowedMovementsRight() - 1);
                                        SetAllowedMovementsLeft(GetAllowedMovementsLeft() + 1);

                                        movementPerformed = String.Concat("se moveu ", y1AxisCounter, " vez(es) no eixo Y");

                                        previousMove = movement;

                                        break;

                                    case "E":
                                        y2AxisCounter++;

                                        SetY(-1);
                                        SetAllowedMovementsLeft(GetAllowedMovementsLeft() - 1);
                                        SetAllowedMovementsRight(GetAllowedMovementsRight() + 1);

                                        movementPerformed = String.Concat("se moveu ", y2AxisCounter, " vez(es) no eixo Y");

                                        previousMove = movement;

                                        break;

                                    case "C":
                                        x1AxisCounter++;

                                        SetX(1);
                                        SetAllowedMovementsUp(GetAllowedMovementsUp() - 1);
                                        SetAllowedMovementsDown(GetAllowedMovementsDown() + 1);

                                        movementPerformed = String.Concat("se moveu ", x1AxisCounter, " vez(es) no eixo X");

                                        previousMove = movement;

                                        break;

                                    case "B":
                                        x2AxisCounter++;

                                        SetX(-1);
                                        SetAllowedMovementsDown(GetAllowedMovementsDown() - 1);
                                        SetAllowedMovementsUp(GetAllowedMovementsUp() + 1);

                                        movementPerformed = String.Concat("se moveu ", x2AxisCounter, " vez(es) no eixo X");

                                        previousMove = movement;

                                        break;

                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                throw new Exception(messageInvalidMove);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(messageInvalidMove);
                    }
                }

                movementsPerformed.Add(movementPerformed);

                List<Object> position = new List<Object>
                {
                    new
                    {
                        x = GetX().ToString()
                    },

                    new
                    {
                        y = GetY().ToString()
                    },

                    new
                    {
                        face = GetFace()
                    },

                    new
                    {
                        movementsPerformed = movementsPerformed
                    }
                };

                Object returnSetPosition = new
                {
                    position
                };

                HttpContext.Current.Response.Write(JavaScriptSerializer(returnSetPosition));
            }
            catch (Exception ex)
            {
                Object returnException = new
                {
                    error = ex.Message
                };

                HttpContext.Current.Response.Write(JavaScriptSerializer(returnException));
            }
        }

        [WebMethod(Description = "Obtém a posição atual da sonda.",
            EnableSession = true)]
        public void GetPosition()
        {
            HttpContext.Current.Response.ContentType = "application/json";

            try
            {
                List<Object> position = new List<Object>
                {
                    new
                    {
                        x = GetX().ToString()
                    },

                    new
                    {
                        y = GetY().ToString()
                    },

                    new
                    {
                        face = GetFace()
                    }
                };

                Object returnGetPosition = new
                {
                    position
                };

                HttpContext.Current.Response.Write(JavaScriptSerializer(returnGetPosition));
            }
            catch (Exception ex)
            {
                Object returnException = new
                {
                    error = ex.Message
                };

                HttpContext.Current.Response.Write(JavaScriptSerializer(returnException));
            }
        }

        #endregion Public WebMethod

        #region Private Methods

        /// <summary>
        /// Reinicia as variaveis de "sessão" e a posição inicial da sonda.
        /// </summary>
        private void ResetSessionVariables()
        {
            // A sonda inicia no quadrante(x = 0, y = 0), o que se traduz como a casa mais inferior da esquerda
            // Também inicia com a face para a direita
            // Se pudéssemos visualizar a posição inicial, seria:
            // -------------------------------
            // | 0,4 | 1,4 | 2,4 | 3,4 | 4,4 |
            // -------------------------------
            // | 0,3 | 1,3 | 2,3 | 3,3 | 4,3 |
            // -------------------------------
            // | 0,2 | 1,2 | 2,2 | 3,2 | 4,2 |
            // -------------------------------
            // | 0,1 | 1,1 | 2,1 | 3,1 | 4,1 |
            // -------------------------------
            // | * > | 1,0 | 2,0 | 3,0 | 4,0 |
            // -------------------------------

            if (Session != null)
            {
                Session.RemoveAll();
            }
            
            Session["x"] = 0;           // Up
            Session["y"] = 0;           // Right
            Session["face"] = "D";      // D = Direita | E = Esquerda | C = Cima | B = Baixo

            // Valores setados considerando um espaço de 5 x 5
            Session["allowedMovementsRight"] = 4;
            Session["allowedMovementsLeft"] = 0;
            Session["allowedMovementsUp"] = 4;
            Session["allowedMovementsDown"] = 0;
        }

        /// <summary>
        /// Verifica se o movimento recebido é um movimento válido ou permitido.
        /// </summary>
        /// <param name="movement"></param>
        /// <returns>bool</returns>
        private bool CheckValidMovement(string movement)
        {
            switch (movement)
            {
                case "GE":  // Girar 90 graus à esquerda
                    return true;

                case "GD":  // Girar 90 graus à direta
                    return true;

                case "M":   // Movimentar
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Verifica se a sonda pode se mover para o "quadrado" solicitado.
        /// </summary>
        /// <param name="movement"></param>
        /// <returns>bool</returns>
        private bool CanMove()
        {
            bool canMove = false;

            switch (GetFace())
            {
                case "D":
                    if (GetAllowedMovementsRight() > 0) canMove = true;
                    break;

                case "E":
                    if (GetAllowedMovementsLeft() > 0) canMove = true;
                    break;

                case "C":
                    if (GetAllowedMovementsUp() > 0) canMove = true;
                    break;

                case "B":
                    if (GetAllowedMovementsDown() > 0) canMove = true;
                    break;

                default:
                    break;
            }

            return canMove;
        }

        /// <summary>
        /// Atribui o novo valor do eixo X após o movimento.
        /// </summary>
        /// <param name="value"></param>
        private void SetX(int value)
        {
            int x = 0;

            if (Session["x"] != null)
            {
                x = (int)Session["x"];
            }

            x = x + (1 * value);

            Session["x"] = x;
        }

        /// <summary>
        /// Obtém o valor atual do eixo X.
        /// </summary>
        /// <returns>int</returns>
        private int GetX()
        {
            int x = 0;

            if (Session["x"] != null)
            {
                x = (int)Session["x"];
            }
            else
            {
                Session["x"] = x;
            }

            return x;
        }

        /// <summary>
        /// Atribui o novo valor do eixo Y após o movimento.
        /// </summary>
        /// <param name="value"></param>
        private void SetY(int value)
        {
            int y = 0;

            if (Session["y"] != null)
            {
                y = (int)Session["y"];
            }

            y = y + (1 * value);

            Session["y"] = y;
        }

        /// <summary>
        /// Obtém o valor atual do eixo Y.
        /// </summary>
        /// <returns>int</returns>
        private int GetY()
        {
            int y = 0;

            if (Session["y"] != null)
            {
                y = (int)Session["y"];
            }
            else
            {
                Session["y"] = y;
            }

            return y;
        }

        /// <summary>
        /// Atribui o novo valor da face após o movimento de rotação.
        /// </summary>
        /// <param name="value"></param>
        private void SetFace(string value)
        {
            string face = GetFace();

            switch (value)
            {
                case "GE":
                    if (face.Equals("D"))
                    {
                        Session["face"] = "C";
                    }
                    else if (face.Equals("E"))
                    {
                        Session["face"] = "B";
                    }
                    else if (face.Equals("C"))
                    {
                        Session["face"] = "E";
                    }
                    else if (face.Equals("B"))
                    {
                        Session["face"] = "D";
                    }

                    break;

                case "GD":
                    if (face.Equals("D"))
                    {
                        Session["face"] = "B";
                    }
                    else if (face.Equals("E"))
                    {
                        Session["face"] = "C";
                    }
                    else if (face.Equals("C"))
                    {
                        Session["face"] = "D";
                    }
                    else if (face.Equals("B"))
                    {
                        Session["face"] = "E";
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Obtém o valor atual da face.
        /// </summary>
        /// <returns>string</returns>
        private string GetFace()
        {
            string face = "D";

            if (Session["face"] != null)
            {
                face = Session["face"].ToString();
            }
            else
            {
                Session["face"] = face;
            }

            return face;
        }

        /// <summary>
        /// Atualiza a quantidade de movimentos restantes para a direita
        /// </summary>
        /// <param name="value"></param>
        private void SetAllowedMovementsRight(int value)
        {
            Session["allowedMovementsRight"] = value;
        }

        /// <summary>
        /// Obtém a quantidade de movimentos restantes para a direita
        /// </summary>
        /// <returns>int</returns>
        private int GetAllowedMovementsRight()
        {
            int allowedMovementsRight = 4;

            if (Session["allowedMovementsRight"] != null)
            {
                allowedMovementsRight = (int)Session["allowedMovementsRight"];
            }
            else
            {
                Session["allowedMovementsRight"] = allowedMovementsRight;
            }

            return allowedMovementsRight;
        }

        /// <summary>
        /// Atualiza a quantidade de movimentos restantes para a esquerda
        /// </summary>
        /// <param name="value"></param>
        private void SetAllowedMovementsLeft(int value)
        {
            Session["allowedMovementsLeft"] = value;
        }

        /// <summary>
        /// Obtém a quantidade de movimentos restantes para a esquerda
        /// </summary>
        /// <returns>int</returns>
        private int GetAllowedMovementsLeft()
        {
            int allowedMovementsLeft = 0;

            if (Session["allowedMovementsLeft"] != null)
            {
                allowedMovementsLeft = (int)Session["allowedMovementsLeft"];
            }
            else
            {
                Session["allowedMovementsLeft"] = allowedMovementsLeft;
            }

            return allowedMovementsLeft;
        }

        /// <summary>
        /// Atualiza a quantidade de movimentos restantes para a cima
        /// </summary>
        /// <param name="value"></param>
        private void SetAllowedMovementsUp(int value)
        {
            Session["allowedMovementsUp"] = value;
        }

        /// <summary>
        /// Obtém a quantidade de movimentos restantes para a cima
        /// </summary>
        /// <returns>int</returns>
        private int GetAllowedMovementsUp()
        {
            int allowedMovementsUp = 4;

            if (Session["allowedMovementsUp"] != null)
            {
                allowedMovementsUp = (int)Session["allowedMovementsUp"];
            }
            else
            {
                Session["allowedMovementsUp"] = allowedMovementsUp;
            }

            return allowedMovementsUp;
        }

        /// <summary>
        /// Atualiza a quantidade de movimentos restantes para a baixo
        /// </summary>
        /// <param name="value"></param>
        private void SetAllowedMovementsDown(int value)
        {
            Session["allowedMovementsDown"] = value;
        }

        /// <summary>
        /// Obtém a quantidade de movimentos restantes para a baixo
        /// </summary>
        /// <returns>int</returns>
        private int GetAllowedMovementsDown()
        {
            int allowedMovementsDown = 0;

            if (Session["allowedMovementsDown"] != null)
            {
                allowedMovementsDown = (int)Session["allowedMovementsDown"];
            }
            else
            {
                Session["allowedMovementsDown"] = allowedMovementsDown;
            }

            return allowedMovementsDown;
        }

        /// <summary>
        /// Serializa o objeto de retorno
        /// </summary>
        /// <param name="response"></param>
        /// <returns>string</returns>
        private string JavaScriptSerializer(Object response)
        {
            var scriptSerializer = new JavaScriptSerializer();

            return scriptSerializer.Serialize(response);
        }

        #endregion Private Methods
    }
}
