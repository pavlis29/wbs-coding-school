import { Router } from "express";
import { echo } from "../controllers/ai.ts";
const aiRouter = Router();
aiRouter.post("/echo", echo);
export default aiRouter;
