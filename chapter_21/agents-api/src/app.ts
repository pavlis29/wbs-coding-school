import express from "express";
import aiRouter from "./routes/aiRouter.ts";
import { errorHandler } from "./middlewares/errorHandler.ts";

const app = express();
const port = process.env.PORT ?? 3000;
app.use(express.json());
app.use("/ai", aiRouter);
app.use(errorHandler);
app.listen(port, () => console.log("Server listening on port 3000."));
